namespace jp.zabaglione.ui.core.binding
{
    /// <summary>
    /// Specifies the direction of data flow in a binding
    /// </summary>
    public enum BindingMode
    {
        /// <summary>
        /// Updates the target property when the source property changes
        /// </summary>
        OneWay,

        /// <summary>
        /// Updates both the target property when the source changes and the source property when the target changes
        /// </summary>
        TwoWay,

        /// <summary>
        /// Updates the source property when the target property changes
        /// </summary>
        OneWayToSource,

        /// <summary>
        /// Updates the target property only once when the binding is first established
        /// </summary>
        OneTime
    }
}