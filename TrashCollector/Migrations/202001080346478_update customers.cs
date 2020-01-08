namespace TrashCollector.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class updatecustomers : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Customers", "Zip", c => c.String());
            AlterColumn("dbo.Customers", "PickupDay", c => c.String());
            AlterColumn("dbo.Customers", "ExtraPickupDate", c => c.String());
            AlterColumn("dbo.Customers", "SuspendedStart", c => c.String());
            AlterColumn("dbo.Customers", "SuspendedEnd", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Customers", "SuspendedEnd", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Customers", "SuspendedStart", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Customers", "ExtraPickupDate", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Customers", "PickupDay", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Customers", "Zip", c => c.Int(nullable: false));
        }
    }
}
