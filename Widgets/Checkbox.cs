namespace Thuja.Widgets
{
    public class Checkbox : Button
    {
        public Checkbox(string text, bool isChecked = false, int maxWidth = int.MaxValue) : base(text, maxWidth)
        {
            Checked = isChecked;
        }

        /// <summary>
        /// Является ли этот checkbox выбранным.
        /// </summary>
        public bool Checked { get; set; }

        /// <summary>
        /// Символ, который будет отображаться, когда этот checkbox выбран.
        /// </summary>
        public char Marker { get; set; } = 'x';

        /// <inheritdoc />
        public override void Render(RenderContext context)
        {
            var mark = Checked ? Marker : ' ';
            var prefix = $"[{mark}] ";
            context.PlaceString(prefix, CurrentStyle);
            base.Render(context.Derive((prefix.Length, 0, 0)));
        }

        /// <summary>
        /// Делает так, чтобы клик на чекбокс менял его состояние.
        /// Удобно, когда не требуется какого-то сложного поведения.
        /// </summary>
        /// <returns>Возвращает себя.</returns>
        public Checkbox Auto()
        {
            OnClick(() => Checked = !Checked);
            return this;
        }

        /// <summary>
        /// Сохраняет себя в переданную переменную. Поволяет по полной использовать builder-методы.
        /// </summary>
        /// <returns>Возвращает себя.</returns>
        public Checkbox Save(out Checkbox self)
        {
            self = this;
            return this;
        }
    }
}