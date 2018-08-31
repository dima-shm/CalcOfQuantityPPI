namespace CalcOfQuantityPPI.ViewModels.Request
{
    public class ProfessionViewModel
    {
        public string ProfessionName { get; set; }

        public int EmployeesQuantity { get; set; }

        public QuantityOfPPIViewModel[] QuantityOfPPI { get; set; }  
    }

    public class QuantityOfPPIViewModel
    {
        public string PersonalProtectiveItemName { get; set; }

        public string ProtectionClass { get; set; }

        public int QuantityForOneEmployee { get; set; }

        public int TotalQuantity { get; set; }
    }
}