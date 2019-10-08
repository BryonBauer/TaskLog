using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TaskLog.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Email = table.Column<string>(nullable: false),
                    Password = table.Column<string>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    ProjectID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    ProjectName = table.Column<string>(nullable: false),
                    ProjectDescription = table.Column<string>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    ProjectStatus = table.Column<string>(nullable: false),
                    EstimatedTime = table.Column<int>(nullable: false),
                    hasTask = table.Column<bool>(nullable: false),
                    ProjectCreatorUserId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.ProjectID);
                    table.ForeignKey(
                        name: "FK_Projects_Users_ProjectCreatorUserId",
                        column: x => x.ProjectCreatorUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    TaskID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    TaskName = table.Column<string>(nullable: false),
                    TaskDescription = table.Column<string>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    EstimatedTime = table.Column<int>(nullable: false),
                    hasSubTasks = table.Column<bool>(nullable: false),
                    ParentProjectId = table.Column<int>(nullable: true),
                    TaskCreatorUserId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.TaskID);
                    table.ForeignKey(
                        name: "FK_Tasks_Projects_ParentProjectId",
                        column: x => x.ParentProjectId,
                        principalTable: "Projects",
                        principalColumn: "ProjectID",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Tasks_Users_TaskCreatorUserId",
                        column: x => x.TaskCreatorUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SubTasks",
                columns: table => new
                {
                    SubTaskID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SubTaskName = table.Column<string>(nullable: false),
                    SubTaskDescription = table.Column<string>(nullable: false),
                    DueDate = table.Column<DateTime>(nullable: false),
                    EstimatedTime = table.Column<int>(nullable: false),
                    hasInnerSubTasks = table.Column<bool>(nullable: false),
                    ParentTaskId = table.Column<int>(nullable: true),
                    SubTaskCreatorUserId = table.Column<int>(nullable: true),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    UpdatedAt = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubTasks", x => x.SubTaskID);
                    table.ForeignKey(
                        name: "FK_SubTasks_Tasks_ParentTaskId",
                        column: x => x.ParentTaskId,
                        principalTable: "Tasks",
                        principalColumn: "TaskID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubTasks_Users_SubTaskCreatorUserId",
                        column: x => x.SubTaskCreatorUserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectCreatorUserId",
                table: "Projects",
                column: "ProjectCreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectID",
                table: "Projects",
                column: "ProjectID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubTasks_ParentTaskId",
                table: "SubTasks",
                column: "ParentTaskId");

            migrationBuilder.CreateIndex(
                name: "IX_SubTasks_SubTaskCreatorUserId",
                table: "SubTasks",
                column: "SubTaskCreatorUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ParentProjectId",
                table: "Tasks",
                column: "ParentProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_TaskCreatorUserId",
                table: "Tasks",
                column: "TaskCreatorUserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubTasks");

            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
