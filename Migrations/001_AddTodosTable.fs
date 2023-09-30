module Migrations.AddTodosTable

open FluentMigrator
open FluentMigrator.Postgres

[<Literal>]
let VersionNumber = 001L

[<Migration(VersionNumber)>]
type AddTodosTable() =
    inherit Migration()
    
    override this.Up() =
        this.Create.Table("Todos")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("Text").AsString() |> ignore
        this.Create.Table("Notes")
            .WithColumn("Id").AsInt64().PrimaryKey().Identity()
            .WithColumn("Text").AsString()
            .WithColumn("TodoId").AsInt64().ForeignKey("Todos", "Id")
            |> ignore
            
   
    override this.Down() =
        this.Delete.Table("Todos") |> ignore
        this.Delete.Table("Notes") |> ignore
