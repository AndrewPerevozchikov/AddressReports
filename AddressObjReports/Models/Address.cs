namespace AddressObjReports.Models
{
    /// <summary>
    /// Адреса
    /// </summary>
    public class Address
    {
        /// <summary>
        /// Уровень адресного объекта
        /// </summary>
        public required int Level { get; set; }
        /// <summary>
        /// Наименование уровня адресного объекта
        /// </summary>
        public required string LevelName { get; set; }
        /// <summary>
        /// Наименование типа
        /// </summary>
        public required string TypeName { get; set; }
        /// <summary>
        /// Наименование
        /// </summary>
        public required string Name { get; set; }
    }
}