namespace 電子採購網爬0305.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class count : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BidCases", "傳輸次數", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BidCases", "傳輸次數", c => c.String(maxLength: 50));
        }
    }
}
