using Microsoft.EntityFrameworkCore.Migrations;
using System;

#nullable disable

namespace Cinema.Infrastructure.Migrations
{
    public partial class seedingdataver20 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            var movieTypeActionId = Guid.NewGuid();
            var movieTypeComedyId = Guid.NewGuid();
            var movieTypeDramaId = Guid.NewGuid();

            migrationBuilder.InsertData(
                table: "MovieTypes",
                columns: new[] { "Id", "MovieTypeName", "IsActive" },
                values: new object[,]
                {
                    { movieTypeActionId, "Action", true },
                    { movieTypeComedyId, "Comedy", true },
                    { movieTypeDramaId, "Drama", true }
                });

            var ratePGId = Guid.NewGuid();
            var rateRId = Guid.NewGuid();
            var rateGId = Guid.NewGuid();

            migrationBuilder.InsertData(
                table: "Rates",
                columns: new[] { "Id", "Description", "Code" },
                values: new object[,]
                {
                    { ratePGId, "PG - Parental Guidance", "PG" },
                    { rateRId, "R - Restricted", "R" },
                    { rateGId, "G - General Audience", "G" }
                });

            migrationBuilder.InsertData(
                table: "Movies",
                columns: new[] { "Id", "MovieDuration", "EndTime", "PremiereDate", "Description", "Director", "Image", "Language", "MovieTypeId", "Name", "RateId", "Trailer", "IsActive" },
                values: new object[,]
                {
                    {
                        Guid.NewGuid(), 120, DateTime.Now.AddMonths(1), DateTime.Now.AddMonths(-1), "An epic action movie with stunning visuals.", "John Doe", "action_movie.jpg", "English",
                        movieTypeActionId, "Action PG movie", ratePGId, "trailer_link_1.mp4", true
                    },
                    {
                        Guid.NewGuid(), 90, DateTime.Now.AddMonths(2), DateTime.Now.AddMonths(-2), "A hilarious comedy that will make you laugh out loud.", "Jane Smith", "comedy_movie.jpg", "English",
                        movieTypeComedyId, "Comedy R movie", rateRId, "trailer_link_2.mp4", true
                    },
                    {
                        Guid.NewGuid(), 110, DateTime.Now.AddMonths(3), DateTime.Now.AddMonths(-3), "A dramatic film with a gripping storyline.", "Alice Johnson", "drama_movie.jpg", "English",
                        movieTypeDramaId, "Drama G movie", rateGId, "trailer_link_3.mp4", true
                    }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
