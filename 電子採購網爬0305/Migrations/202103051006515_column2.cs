namespace 電子採購網爬0305.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class column2 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BidCases", "標案案號標案名稱", c => c.String());
            DropColumn("dbo.BidCases", "標案案號");
            DropColumn("dbo.BidCases", "標案名稱");
        }
        
        public override void Down()
        {
            AddColumn("dbo.BidCases", "標案名稱", c => c.String(maxLength: 200));
            AddColumn("dbo.BidCases", "標案案號", c => c.String(maxLength: 100));
            DropColumn("dbo.BidCases", "標案案號標案名稱");
        }
    }
}
