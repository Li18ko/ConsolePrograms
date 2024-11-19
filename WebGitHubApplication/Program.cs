using GitHubApiClient;

namespace WebGitHubApplication {
    public class Program {
        public static void Main(string[] args) {
            var builder = WebApplication.CreateBuilder(args);
            string githubToken = builder.Configuration["GitHubApi:Token"];
            builder.Services.AddSingleton<IGitHubClient>(new GitHubClient(githubToken));
            builder.Services.AddControllers();
            
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
            
            var app = builder.Build();
            
            if (app.Environment.IsDevelopment()) {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            
            app.UseMiddleware<ErrorHandlingMiddleware>();
            app.UseHttpsRedirection();
            app.MapControllers();

            app.Run();
        }

    }

}
