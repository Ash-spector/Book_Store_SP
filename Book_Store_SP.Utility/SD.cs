namespace Book_Store_SP.Utility
{
    public static class SD
    {
        // Roles
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";
        public const string Role_Company = "Company";
        public const string Role_Individual = "Individual";

        // Category SPs
        public const string Proc_Category_Create = "Category";
        public const string Proc_Category_Update = "UpdateCategory";
        public const string Proc_Category_Delete = "DeleteCategory";
        public const string Proc_Category_GetOne = "GetCategory";
        public const string Proc_Category_GetAll = "GetCategories";

        // CoverType SPs
        public const string Proc_CoverType_Create = "CoverType";
        public const string Proc_CoverType_Update = "UpdateCoverType";
        public const string Proc_CoverType_Delete = "DeleteCoverType";
        public const string Proc_CoverType_GetOne = "GetCoverType";
        public const string Proc_CoverType_GetAll = "GetCoverTypes";

        // Product SPs
        public const string Proc_Product_Create = "Product";
        public const string Proc_Product_Update = "UpdateProduct";
        public const string Proc_Product_Delete = "DeleteProduct";
        public const string Proc_Product_GetOne = "GetProduct";
        public const string Proc_Product_GetAll = "GetProducts";

        // Company SPs
        public const string Proc_Company_Create = "Company";
        public const string Proc_Company_Update = "UpdateCompany";
        public const string Proc_Company_Delete = "DeleteCompany";
        public const string Proc_Company_GetOne = "GetCompany";
        public const string Proc_Company_GetAll = "GetCompanies";
    }
}