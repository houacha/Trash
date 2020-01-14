namespace TrashCollector.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class madelatandlngfindinstart : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Customers", "Lat", c => c.String());
            AddColumn("dbo.Customers", "Lng", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "Lng");
            DropColumn("dbo.Customers", "Lat");
        }
    }
}
