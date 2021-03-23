namespace 電子採購網爬0305.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class datetime1 : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BidCases", "公告日期", c => c.DateTime(nullable: false));
            AlterColumn("dbo.BidCases", "截止投標", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BidCases", "截止投標", c => c.DateTime());
            AlterColumn("dbo.BidCases", "公告日期", c => c.DateTime());
        }
    }
}
