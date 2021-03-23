namespace 電子採購網爬0305.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class create : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.BidCases",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        項次 = c.String(),
                        機關名稱 = c.String(maxLength: 100),
                        標案案號 = c.String(maxLength: 100),
                        標案名稱 = c.String(maxLength: 200),
                        傳輸次數 = c.String(maxLength: 50),
                        招標方式 = c.String(maxLength: 100),
                        採購性質 = c.String(maxLength: 50),
                        公告日期 = c.DateTime(),
                        截止投標 = c.DateTime(),
                        預算金額 = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.BidCases");
        }
    }
}
