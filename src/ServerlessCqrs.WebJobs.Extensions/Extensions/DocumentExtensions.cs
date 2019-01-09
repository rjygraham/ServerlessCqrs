//using Microsoft.Azure.Documents;
//using Newtonsoft.Json;
//using System.IO;
//using System.Threading.Tasks;

//namespace ServerlessCqrs.WebJobs.Extensions.Extensions
//{
//	public static class DocumentExtensions
//	{
//		private static JsonSerializerSettings settings = new JsonSerializerSettings
//		{
//			TypeNameHandling = TypeNameHandling.Objects
//		};

//		/// <summary>
//		/// Handy extension method to convert from a Document to strongly-typed class. Found on StackOverflow:
//		/// https://stackoverflow.com/questions/27118998/converting-created-document-result-to-poco/47188631#47188631
//		/// </summary>
//		/// <typeparam name="T"></typeparam>
//		/// <param name="document">Microsoft.Azure.Documents.Document returned in the CosmosDB Trigger.</param>
//		/// <returns>Instance of the type specified in the type info.</returns>
//		public static async Task<T> ReadAsAsync<T>(this Document document)
//		{
//			using (var ms = new MemoryStream())
//			{
//				using (var reader = new StreamReader(ms))
//				{
//					document.SaveTo(ms);
//					ms.Position = 0;
//					return JsonConvert.DeserializeObject<T>(await reader.ReadToEndAsync(), settings);
//				}
//			}
//		}


//		public static string ReadAsString(this Document document)
//		{
//			using (var ms = new MemoryStream())
//			{
//				using (var reader = new StreamReader(ms))
//				{
//					document.SaveTo(ms);
//					ms.Position = 0;
//					return reader.ReadToEnd();
//				}
//			}
//		}
//	}
//}
