namespace TrashCollector.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addcustomerpickupconfirm : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "PickupConfirmed", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "PickupConfirmed");
        }
    }
}
