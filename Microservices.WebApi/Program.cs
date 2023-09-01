using Microservices.WebApi;
using Microsoft.Extensions.Configuration;
using Microsoft.OpenApi.Models;
using MMLib.SwaggerForOcelot.DependencyInjection;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Values;

var builder = WebApplication.CreateBuilder(args);



//builder.Services.AddSwaggerForOcelot(builder.Configuration);
//builder.Services.AddSwaggerForOcelot(builder.Configuration,
  
    //Gera a documentação swagger do gateway
    //(o) =>  {      o.GenerateDocsForGatewayItSelf = true;  }
//);


builder.Services.AddSwaggerForOcelot(builder.Configuration,
  (o) =>
  {
      o.GenerateDocsDocsForGatewayItSelf(opt =>
      {
          opt.FilePathsForXmlComments = new string[] { "MyAPI.xml" };
          opt.GatewayDocsTitle = "My Gateway";
          opt.GatewayDocsOpenApiInfo = new()
          {
              Title = "My Gateway",
              Version = "v1",
          };
         // opt.DocumentFilter<MyDocumentFilter>();
          opt.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
          {
              Description = @"JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below. Example: 'Bearer 12345abcdef'",
              Name = "Authorization",
              In = ParameterLocation.Header,
              Type = SecuritySchemeType.ApiKey,
              Scheme = "Bearer"
          });
          opt.AddSecurityRequirement(new OpenApiSecurityRequirement()
          {
              {
                  new OpenApiSecurityScheme
                  {
                      Reference = new OpenApiReference
                      {
                          Type = ReferenceType.SecurityScheme,
                          Id = "Bearer"
                      },
                      Scheme = "oauth2",
                      Name = "Bearer",
                      In = ParameterLocation.Header,
                  },
                  new List<string>()
              }
          });
      });
  });






builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen();


builder.Configuration.AddJsonFile("ocelot.json", optional: false, reloadOnChange: true);
builder.Services.AddOcelot(builder.Configuration);
var app = builder.Build();


app.UseSwaggerForOcelotUI(opt =>
{
    opt.PathToSwaggerGenerator = "/swagger/docs";

    opt.DownstreamSwaggerHeaders = new[]
    {
      new KeyValuePair<string, string>("Auth-Key", "AuthValue"),
    };
});

     // app.UseSwagger();
    // app.UseSwaggerUI();





app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers(); 



await app.UseOcelot();

app.Run();
