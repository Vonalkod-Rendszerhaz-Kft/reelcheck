namespace ReelCheck.Core.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddReeelstable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Reels",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        USERNAME = c.String(maxLength: 50),
                        STATION_ID = c.String(maxLength: 10),
                        STATION_PROCESS = c.String(maxLength: 10),
                        IPPORT_ID = c.String(maxLength: 25),
                        IPPORT_CHECK = c.String(maxLength: 25),
                        IPPORT_PRN = c.String(maxLength: 25),
                        TIMESTAMP_START = c.DateTime(nullable: false),
                        TIMESTAMP_END = c.DateTime(),
                        RESULT = c.String(maxLength: 4),
                        RESULT_ID = c.String(maxLength: 6),
                        RESULT_IDCONF = c.String(maxLength: 4),
                        RESULT_CHECK = c.String(maxLength: 6),
                        ATTEMPTS_READID = c.Int(nullable: false),
                        ATTEMPTS_READIDHH = c.Int(name: "ATTEMPTS_ READIDHH", nullable: false),
                        ATTEMPTS_READCHECK = c.Int(nullable: false),
                        ATTEMPTS_READCHECKHH = c.Int(nullable: false),
                        ATTEMPTS_PRINT = c.Int(nullable: false),
                        READERDATA_ID = c.String(),
                        READERDATA_CHECK = c.String(),
                        MSGDATA_IDDATA = c.String(),
                        MSGDATA_IDDATAACK = c.String(),
                        DE_LABELID = c.String(maxLength: 100),
                        DE_SVENDOR = c.String(maxLength: 100),
                        DE_SPN = c.String(maxLength: 100),
                        DE_SLOT = c.String(maxLength: 100),
                        DE_SFVS = c.String(maxLength: 100),
                        DE_SRSN = c.String(maxLength: 100),
                        DE_SQTY = c.String(maxLength: 100),
                        DE_IMTSID = c.String(maxLength: 100),
                        DE_IFVS = c.String(maxLength: 100),
                        DE_IQTY = c.String(maxLength: 100),
                        DE_ILOT = c.String(maxLength: 100),
                        DE_IPN = c.String(maxLength: 100),
                        DE_IVENDOR = c.String(maxLength: 100),
                    })
                .PrimaryKey(t => t.Id)
                .Index(t => t.USERNAME)
                .Index(t => t.STATION_ID)
                .Index(t => t.STATION_PROCESS)
                .Index(t => t.IPPORT_ID)
                .Index(t => t.IPPORT_CHECK)
                .Index(t => t.IPPORT_PRN)
                .Index(t => t.TIMESTAMP_START)
                .Index(t => t.TIMESTAMP_END)
                .Index(t => t.RESULT)
                .Index(t => t.RESULT_ID)
                .Index(t => t.RESULT_IDCONF)
                .Index(t => t.RESULT_CHECK)
                .Index(t => t.DE_LABELID)
                .Index(t => t.DE_SVENDOR)
                .Index(t => t.DE_SPN)
                .Index(t => t.DE_SLOT)
                .Index(t => t.DE_SFVS)
                .Index(t => t.DE_SRSN)
                .Index(t => t.DE_SQTY)
                .Index(t => t.DE_IMTSID)
                .Index(t => t.DE_IFVS)
                .Index(t => t.DE_IQTY)
                .Index(t => t.DE_ILOT)
                .Index(t => t.DE_IPN)
                .Index(t => t.DE_IVENDOR);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Reels", new[] { "DE_IVENDOR" });
            DropIndex("dbo.Reels", new[] { "DE_IPN" });
            DropIndex("dbo.Reels", new[] { "DE_ILOT" });
            DropIndex("dbo.Reels", new[] { "DE_IQTY" });
            DropIndex("dbo.Reels", new[] { "DE_IFVS" });
            DropIndex("dbo.Reels", new[] { "DE_IMTSID" });
            DropIndex("dbo.Reels", new[] { "DE_SQTY" });
            DropIndex("dbo.Reels", new[] { "DE_SRSN" });
            DropIndex("dbo.Reels", new[] { "DE_SFVS" });
            DropIndex("dbo.Reels", new[] { "DE_SLOT" });
            DropIndex("dbo.Reels", new[] { "DE_SPN" });
            DropIndex("dbo.Reels", new[] { "DE_SVENDOR" });
            DropIndex("dbo.Reels", new[] { "DE_LABELID" });
            DropIndex("dbo.Reels", new[] { "RESULT_CHECK" });
            DropIndex("dbo.Reels", new[] { "RESULT_IDCONF" });
            DropIndex("dbo.Reels", new[] { "RESULT_ID" });
            DropIndex("dbo.Reels", new[] { "RESULT" });
            DropIndex("dbo.Reels", new[] { "TIMESTAMP_END" });
            DropIndex("dbo.Reels", new[] { "TIMESTAMP_START" });
            DropIndex("dbo.Reels", new[] { "IPPORT_PRN" });
            DropIndex("dbo.Reels", new[] { "IPPORT_CHECK" });
            DropIndex("dbo.Reels", new[] { "IPPORT_ID" });
            DropIndex("dbo.Reels", new[] { "STATION_PROCESS" });
            DropIndex("dbo.Reels", new[] { "STATION_ID" });
            DropIndex("dbo.Reels", new[] { "USERNAME" });
            DropTable("dbo.Reels");
        }
    }
}
