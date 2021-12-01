namespace ReelCheck.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Increasestatuscolumnslength : DbMigration
    {
        public override void Up()
        {
            DropIndex("dbo.Reels", new[] { "RESULT" });
            DropIndex("dbo.Reels", new[] { "RESULT_ID" });
            DropIndex("dbo.Reels", new[] { "RESULT_IDCONF" });
            DropIndex("dbo.Reels", new[] { "RESULT_CHECK" });
            AlterColumn("dbo.Reels", "RESULT", c => c.String(maxLength: 10));
            AlterColumn("dbo.Reels", "RESULT_ID", c => c.String(maxLength: 10));
            AlterColumn("dbo.Reels", "RESULT_IDCONF", c => c.String(maxLength: 10));
            AlterColumn("dbo.Reels", "RESULT_CHECK", c => c.String(maxLength: 10));
            CreateIndex("dbo.Reels", "RESULT");
            CreateIndex("dbo.Reels", "RESULT_ID");
            CreateIndex("dbo.Reels", "RESULT_IDCONF");
            CreateIndex("dbo.Reels", "RESULT_CHECK");
        }
        
        public override void Down()
        {
            DropIndex("dbo.Reels", new[] { "RESULT_CHECK" });
            DropIndex("dbo.Reels", new[] { "RESULT_IDCONF" });
            DropIndex("dbo.Reels", new[] { "RESULT_ID" });
            DropIndex("dbo.Reels", new[] { "RESULT" });
            AlterColumn("dbo.Reels", "RESULT_CHECK", c => c.String(maxLength: 6));
            AlterColumn("dbo.Reels", "RESULT_IDCONF", c => c.String(maxLength: 4));
            AlterColumn("dbo.Reels", "RESULT_ID", c => c.String(maxLength: 6));
            AlterColumn("dbo.Reels", "RESULT", c => c.String(maxLength: 4));
            CreateIndex("dbo.Reels", "RESULT_CHECK");
            CreateIndex("dbo.Reels", "RESULT_IDCONF");
            CreateIndex("dbo.Reels", "RESULT_ID");
            CreateIndex("dbo.Reels", "RESULT");
        }
    }
}
