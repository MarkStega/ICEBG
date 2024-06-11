namespace Materia.Blazor
{
    public record MBDialogButton
    {
        #region members

        public string ButtonLabel { get; set; }
        public MBButtonStyle ButtonStyle { get; set; }
        public string ButtonValue { get; set; }

        #endregion
    }
}
