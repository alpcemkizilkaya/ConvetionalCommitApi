using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LibGit2Sharp;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ConvetionalCommitApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class CommitController : ControllerBase
    {

        private readonly ILogger<CommitController> _logger;

        public CommitController(ILogger<CommitController> logger)
        {
            _logger = logger;
        }

        [HttpGet]
        public List<Commit> Get(String path,String username,String password,String branch)
        {   
            DirectoryInfo di = new DirectoryInfo("tempRepo/");
            if (di.Exists)
            {
                BranchController.setAttributesNormal(di);
                di.Delete(true);
            }
            string clone ;
            if (username != null && password != null)
            {
                var cloneOptions = new CloneOptions()
                {
                    CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                    {
                        Username = username,
                        Password = password
                    }
                };
                clone = Repository.Clone(path, "tempRepo/", cloneOptions);
            }
            else
            {
                clone = Repository.Clone(path, "tempRepo/");
            }
            var list = new List<Commit>();
            using (var repo = new Repository(@clone))
            {
                // Object lookup
                var obj = repo.Lookup("sha");
                var commit = repo.Lookup<LibGit2Sharp.Commit>("sha");
                var tree = repo.Lookup<Tree>("sha");
                // var tag = repo.Lookup<Tag>("sha");

                ICommitLog commits = null;
                
                if (branch == null)
                {
                    commits = repo.Head.Commits;
                }
                else
                {
                    commits = repo.Branches[branch].Commits;
                }
                
                foreach (var tempCommit in commits.Take(100))
                {
                    string message;
                    string type = "None";
                    if (tempCommit.Message.Contains(":"))
                    {
                        string[] strings = tempCommit.Message.Split(":");
                        message = strings[1];
                        type = strings[0];
                    }
                    else
                    {
                        message = tempCommit.Message;
                    }

                    var commitObj = new Commit
                    {
                        author = tempCommit.Author, commiter = tempCommit.Committer, message = message,type = type
                    };
                    list.Add(commitObj);
                }

                return list;

                // Branches
                // special kind of reference
                



            }
        }
    }
}