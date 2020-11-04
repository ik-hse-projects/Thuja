import argparse
import dataclasses
import ctypes
from sys import stdout, stderr
from dataclasses import dataclass
from PIL import Image
import progressbar 

import caca
from caca import _lib as caca_lib
from caca.canvas import Canvas, CanvasError
from caca.dither import Dither, DitherError

@dataclass(frozen=True)
class Settings:
    img_size: (int, int)
    text_size: (int, int)
    fps: (int, int)
    alpha: bool

    @property
    def total_chars(self) -> str:
        return self.text_size[0] * self.text_size[1]

    @property
    def im_mode(self) -> str:
        if self.alpha:
            return 'RGBA'
        else:
            return 'RGB'

    @property
    def depth(self) -> int:
        if self.alpha:
            return 4
        else:
            return 3

    @property
    def bpp(self) -> int:
        return self.depth * 8

    @property
    def masks(self) -> (int, int, int, int):
        red   = 0x000000ff
        green = 0x0000ff00
        blue  = 0x00ff0000
        alpha = 0xff000000
        if not self.alpha:
            alpha = 0
        return red, green, blue, alpha

    def save(self):
        width, height = self.text_size
        fps1, fps2 = self.fps
        assert width < (1<<16)
        assert height < (1<<16)
        assert fps1 < 256
        assert fps2 < 256
        return bytearray([
            (width & 0x00FF),
            (width & 0xFF00) >> 8,
            (height & 0x00FF),
            (height & 0xFF00) >> 8,
            fps1,
            fps2,
        ])


def is_correct_color(c):
    return 0 <= c <= 16 or c == 32


class CharInfo:
    def __init__(self, foreground, background, flags, char):
        assert is_correct_color(foreground)
        self.foreground = foreground
        assert is_correct_color(background)
        self.background = background
        self.flags = flags
        self.char = char

    def save(self) -> bytes:
        first = 0

        if self.foreground == 32:
            self.foreground = 17
        if self.background == 32:
            self.background = 17
        assert self.foreground < (1 << 5)
        assert self.background < (1 << 5)
        assert self.flags < (1 << 6)

        first = (
            (self.foreground << 11)
            | (self.background << 6)
            | self.flags
        )

        assert self.char < (1 << 24)
        result = [
            (first & 0x00FF),
            (first & 0xFF00) >> 8,
            (self.char & 0x0000FF),
            (self.char & 0x00FF00) >> 8,
            (self.char & 0xFF0000) >> 16,
        ]
        return result


def load_char(attr, char):
    return CharInfo(
        foreground = caca.attr_to_ansi_fg(attr),
        background = caca.attr_to_ansi_bg(attr),
        flags = attr & 0b1111,
        char = ord(char),
    )


bpc = 5
class State:
    def __init__(self, settings):
        red, green, blue, alpha = settings.masks
        self.dit = Dither(
            settings.bpp,
            settings.img_size[0],
            settings.img_size[1],
            settings.depth * settings.img_size[0],
            red, green, blue, alpha
        )
        self.dit.set_algorithm(b"fstein")
        self.dit.set_charset(b"ascii")

        self.canvas = Canvas(settings.text_size[0], settings.text_size[1])
        self.canvas.set_color_ansi(caca.COLOR_DEFAULT, caca.COLOR_TRANSPARENT)

        self.settings = settings
        self._buffer = bytearray([0] * (bpc * self.settings.total_chars))

        caca_lib.caca_get_canvas_attrs.argtypes = [caca.canvas._Canvas]
        caca_lib.caca_get_canvas_attrs.restype = ctypes.POINTER(ctypes.c_uint32*settings.total_chars)
        caca_lib.caca_get_canvas_chars.argtypes = [caca.canvas._Canvas]
        caca_lib.caca_get_canvas_chars.restype = ctypes.POINTER((ctypes.c_uint8*4)*settings.total_chars)

    def add_frame(self, path, display=False):
        img = Image.open(path)
        if img.mode != self.settings.im_mode:
            img = img.convert(self.settings.im_mode)
        assert img.size == self.settings.img_size

        self.dit.bitmap(
            self.canvas,
            0, 0,
            self.settings.text_size[0], self.settings.text_size[1],
            img.tobytes()
        )
        if display:
            stderr.write("%s" % self.canvas.export_to_memory("utf8"))
            stderr.write('\n' * self.settings.text_size[1])

        offset = 0
        attrs = caca_lib.caca_get_canvas_attrs(self.canvas).contents
        chars = caca_lib.caca_get_canvas_chars(self.canvas).contents
        for i in range(self.settings.total_chars):
            attr = attrs[i]
            char = chars[i]
            char = bytearray(char).decode('utf-32-le')
            info = load_char(attr, char)
            encoded = info.save()
            assert len(encoded) == bpc
            for b in encoded:
                self._buffer[offset] = b
                offset += 1

        self.canvas.clear()
        return self._buffer

def find_size(original, maximum):
    result = []
    if maximum[0] is not None:
        found = False
        for width in range(maximum[0], 1, -1):
            height = (original[1] * width) // original[0]
            if (original[1] * width) % original[0] == 0:
                if maximum[1] is not None and height <= maximum[1]:
                    found = True
                    break
        if found:
            result.append((width, height))
    if maximum[1] is not None:
        found = False
        for height in range(maximum[1], 1, -1):
            width = (original[0] * height) // original[1]
            if (original[0] * height) % original[1] == 0:
                if maximum[0] is not None and width <= maximum[0]:
                    found = True
                    break
        if found:
            result.append((width, height))
    assert all(original[0] / original[1] == i[0] / i[1] for i in result)
    return result


def parse_fps(fps):
    splitted = fps.split('/')
    if len(splitted) == 0:
        return 0, 0
    if len(splitted) == 1:
        return int(splitted[0]), 1
    if len(splitted) == 2:
        return int(splitted[0]), int(splitted[1])
    raise Exception('Invalid fps value')


def write_file(settings, frames, out, display=False):
    state = State(settings)
    out.write(settings.save())
    widgets = [
        'Rendering ', progressbar.SimpleProgress(),
        ' (', progressbar.Percentage(), ')',
        ' | ', progressbar.AdaptiveETA(),
    ]
    bar = progressbar.ProgressBar(widgets=widgets)
    for i in bar(frames):
        out.write(state.add_frame(i, display=display))


def main():
    parser = argparse.ArgumentParser(description='Process some integers.')
    parser.add_argument('--width', type=int)
    parser.add_argument('--height', type=int)
    parser.add_argument('--force', action='store_true')
    parser.add_argument('--fps', type=str, default='0/0')
    parser.add_argument('--alpha', default=None, action='store_true')
    parser.add_argument('--no-alpha', dest='alpha', action='store_false')
    parser.add_argument('--out', required=True, type=str)
    parser.add_argument('--display', action='store_true')
    parser.add_argument('frames', nargs='+')
    args = parser.parse_args()

    if args.width is None and args.height is None:
        print('Falling back to 80x25 terminal', file=stderr)
        args.width = 80
        args.height = 25

    first = Image.open(args.frames[0])

    if args.force:
        if args.width is None:
            text_size = (
                round(args.height * first.size[0] / first.size[1]),
                args.height
            )
        elif args.height is None:
            text_size = (
                args.width,
                round(args.width * first.size[1] / first.size[0]),
            )
        else:
            text_size = (args.width, args.height)
    else:
        sizes = find_size(first.size, (args.width, args.height))
        text_size = max(sizes, key=lambda x: x[0] * x[1])

    if args.alpha is None:
        args.alpha = first.mode == 'RGBA'
    settings = Settings(
        img_size = first.size,
        text_size = text_size,
        alpha = args.alpha,
        fps = parse_fps(args.fps)
    )
    if settings.fps == (0, 0):
        print("Warning: you didn't specified fps", file=stderr)
    print(settings, file=stderr)

    if args.display:
        print('Start?', file=stderr)
        input()
    if args.out == '-':
        write_file(settings, args.frames, stdout.buffer, args.display)
    else:
        with open(args.out, 'wb') as f:
            write_file(settings, args.frames, f, args.display)


if __name__ == '__main__':
    main()

