using EDI.Contracts.Repository;
using EDI.DataAccess.Context;
using EDI.Repositories.ImplementRepositories;
using Microsoft.Azure.Cosmos;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace EDI.Services
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy",
                    builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EDI.Services", Version = "v1" });
            });

            services.AddDbContext<CosmosContext>(options =>
                options.UseCosmos(
                    Configuration["CosmosDb:Account"],
                    Configuration["CosmosDb:Key"],
                    Configuration["CosmosDb:DatabaseName"]
                )
            );

            services.AddSingleton<IEDIRepository>(InitializeCosmosClientInstanceAsync(Configuration.GetSection("CosmosDb")).GetAwaiter().GetResult());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EDI.Services v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("CorsPolicy");

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private static async Task<EDIRepository> InitializeCosmosClientInstanceAsync(IConfigurationSection configurationSection)
        {
            //Busca valores deen appsetting
            string databaseName = configurationSection.GetSection("DatabaseName").Value;
            string containerName = configurationSection.GetSection("ContainerName").Value;
            string account = configurationSection.GetSection("Account").Value;
            string key = configurationSection.GetSection("Key").Value;

            //Instancia el controlador de cosmosDB 
            CosmosClient client = new CosmosClient(account, key);

            //Crea la instancia del objeto CosmosDbService 
            EDIRepository cosmosDbService = new EDIRepository(client, databaseName, containerName);

            //Define el objeto database que manipulara el client
            DatabaseResponse database = await client.CreateDatabaseIfNotExistsAsync(databaseName);
            await database.Database.CreateContainerIfNotExistsAsync(containerName, "/id");

            // Retorna la instancia del objeto cosmosDbService
            return cosmosDbService;
        }
    }
}
