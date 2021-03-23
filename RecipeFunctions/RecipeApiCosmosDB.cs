using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using RecipeFunctions.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace RecipeFunctions
{
    public static class RecipeApiCosmosDB
    {
        private const string Route = "Recipe";
        private const string DatabaseName = "Recipe";
        private const string CollectionName = "Recipe";

        [FunctionName("CreateRecipe")]
        public static async Task<IActionResult> CreateRecipe(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = Route)] HttpRequest req,
            [CosmosDB(
                DatabaseName,
                CollectionName,
                ConnectionStringSetting = "CosmosDBConnection")]
            IAsyncCollector<object> recipes, ILogger log)
        {
            log.LogInformation("Creating a new Recipe");
            var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var input = JsonConvert.DeserializeObject<Recipe>(requestBody);

            var recipe = new Recipe() 
            {
                RecipeName = input.RecipeName, 
                Ingredients = input.Ingredients, 
                Steps = input.Steps, 
                PrepTimeMinutes = input.PrepTimeMinutes,
                CookTimeMinutes = input.CookTimeMinutes,
                ReadyInMinutes = input.ReadyInMinutes,
                Creator = input.Creator,
                Notes = input.Notes
            };
            //the object we need to add has to have a lower case id property or we'll
            // end up with a cosmosdb document with two properties - id (autogenerated) and Id
            await recipes.AddAsync(new { id = recipe.Id,
                RecipeName = recipe.RecipeName,
                Ingredients = recipe.Ingredients,
                Steps = recipe.Steps,
                PrepTimeMinutes = recipe.PrepTimeMinutes,
                CookTimeMinutes = recipe.CookTimeMinutes,
                ReadyInMinutes = recipe.ReadyInMinutes,
                Creator = recipe.Creator,
                Notes = recipe.Notes
            });
            return new OkObjectResult(recipe);
        }

        [FunctionName("GetRecipes")]
        public static IActionResult GetRecipes(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Route)] HttpRequest req,
            [CosmosDB(
                DatabaseName,
                CollectionName,
                ConnectionStringSetting = "CosmosDBConnection",
                SqlQuery = "SELECT * FROM c order by c._ts desc")]
                IEnumerable<Recipe> recipes,
            ILogger log)
        {
            log.LogInformation("Getting recipe list items");
            return new OkObjectResult(recipes);
        }

        [FunctionName("GetRecipeById")]
        public static IActionResult GetRecipeById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = Route + "/{id}")] HttpRequest req,
            [CosmosDB(DatabaseName, CollectionName, ConnectionStringSetting = "CosmosDBConnection",
                Id = "{id}")] Recipe recipe,
            ILogger log, string id)
        {
            log.LogInformation("Getting recipe item by id");

            if (recipe == null)
            {
                log.LogInformation($"Item {id} not found");
                return new NotFoundResult();
            }
            return new OkObjectResult(recipe);
        }

        [FunctionName("UpdateRecipe")]
        public static async Task<IActionResult> UpdateRecipe(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = Route + "/{id}")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "CosmosDBConnection")]
                DocumentClient client,
            ILogger log, string id)
        {
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            var updated = JsonConvert.DeserializeObject<Recipe>(requestBody);
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName);
            var document = client.CreateDocumentQuery(collectionUri).Where(t => t.Id == id)
                            .AsEnumerable().FirstOrDefault();
            if (document == null)
            {
                return new NotFoundResult();
            }

            //TODO Update
            //document.SetPropertyValue("IsCompleted", updated.IsCompleted);
            //if (!string.IsNullOrEmpty(updated.TaskDescription))
            //{
            //    document.SetPropertyValue("TaskDescription", updated.TaskDescription);
            //}

            await client.ReplaceDocumentAsync(document);

            /* var todo = new Todo()
            {
                Id = document.GetPropertyValue<string>("id"),
                CreatedTime = document.GetPropertyValue<DateTime>("CreatedTime"),
                TaskDescription = document.GetPropertyValue<string>("TaskDescription"),
                IsCompleted = document.GetPropertyValue<bool>("IsCompleted")
            };*/

            // an easier way to deserialize a Document
            Recipe recipe = (dynamic)document;

            return new OkObjectResult(recipe);
        }

        [FunctionName("DeleteRecipe")]
        public static async Task<IActionResult> DeleteRecipe(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = Route + "/{id}")] HttpRequest req,
            [CosmosDB(ConnectionStringSetting = "CosmosDBConnection")] DocumentClient client,
            ILogger log, string id)
        {
            Uri collectionUri = UriFactory.CreateDocumentCollectionUri(DatabaseName, CollectionName);
            var document = client.CreateDocumentQuery(collectionUri).Where(t => t.Id == id)
                    .AsEnumerable().FirstOrDefault();
            if (document == null)
            {
                return new NotFoundResult();
            }
            await client.DeleteDocumentAsync(document.SelfLink);
            return new OkResult();
        }
    }
}

