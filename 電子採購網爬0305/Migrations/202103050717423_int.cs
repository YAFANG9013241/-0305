namespace 電子採購網爬0305.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _int : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.BidCases", "項次", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.BidCases", "項次", c => c.String());
        }
    }
}
