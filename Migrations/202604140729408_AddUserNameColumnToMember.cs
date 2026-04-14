namespace DatabaseQuiz.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddUserNameColumnToMember : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Members", "UserName", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Members", "UserName");
        }
    }
}
