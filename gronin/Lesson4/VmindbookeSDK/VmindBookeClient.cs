using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Polly;
using RestSharp;

namespace VmindbookeSDK
{
    public class VmindBookeClient:IVmindbookeClient
    {
        public IReadOnlyCollection<User> GetUsers()
        {
            var client = new RestClient("http://135.181.101.47");
            var request = new RestRequest("users",Method.GET);

            var retryPolicy = Policy.HandleResult<IRestResponse>
                (r=> !r.IsSuccessful)
                .Retry(3);
            
            
            var response = retryPolicy.Execute(()=> client.Execute(request));
            var content = JsonConvert.DeserializeObject<List<User>>(response.Content);
            return content;
        }

        public string RegisterUser(User user)
        {
            var client = new RestClient("http://135.181.101.47");
            var request = new RestRequest("users",Method.POST);
            request.AddJsonBody(user);
            var response = client.Execute(request);
            var content = JsonConvert.DeserializeObject<UserRegistrationResponse>(response.Content);
            return content.token;
        }

        public string AddPost(Post post,int SenderId)
        {
            var client = new RestClient("http://135.181.101.47");
            var request = new RestRequest($"users/{SenderId}/posts",Method.POST);
            request.AddJsonBody(post);
            request.AddHeader("Authorization", "7b5eba5e60ae4b238f2cf50438e26bf3");
            
            var retryPolicy = Policy.HandleResult<IRestResponse>
                    (r=> !r.IsSuccessful)
                .Retry(10);
            
            
            var response = retryPolicy.Execute(()=> client.Execute(request));
            //var content = JsonConvert.DeserializeObject<int>(response.Content);
            return response.Content;
        }

        public int LikeAll(int SenderId)
        {
            for (int i = 1; i < 100; i++)
            {
                var client = new RestClient("http://135.181.101.47");
                var request = new RestRequest($"/posts/{i}/likes",Method.POST);

                request.AddHeader("Authorization", "7b5eba5e60ae4b238f2cf50438e26bf3");

                var retryPolicy = Policy.HandleResult<IRestResponse>
                        (r => !r.IsSuccessful)
                    .Retry(10);


                var response = retryPolicy.Execute(() => client.Execute(request));
                Console.WriteLine("123");
            }

            //var content = JsonConvert.DeserializeObject<int>(response.Content);
            return 1;
        }
    }

    
}