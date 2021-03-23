namespace 電子採購網爬0305.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class money : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BidCases", "預算金額", c => c.Int());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BidCases", "預算金額", c => c.Int(nullable: false));
        }
    }
}
