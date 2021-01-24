using System;
using System.Collections;
using System.Collections.Generic;
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
        public List<Commit> Get(String path,String username,String password)
        {
            var cloneOptions = new CloneOptions()
            {
                CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials
                {
                    Username = username,
                    Password = password
                }
            };
            var clone = Repository.Clone(path, "tempRepo/", cloneOptions);
            var list = new List<Commit>();
            using (var repo = new Repository(@clone))
            {
                // Object lookup
                var obj = repo.Lookup("sha");
                var commit = repo.Lookup<LibGit2Sharp.Commit>("sha");
                var tree = repo.Lookup<Tree>("sha");
                // var tag = repo.Lookup<Tag>("sha");

                
                foreach (var headCommit in repo.Head.Commits)
                {

                    var commitObj = new Commit
                    {
                        author = headCommit.Author, commiter = headCommit.Committer, message = headCommit.Message
                    };
                    list.Add(commitObj);
                }
                System.IO.DirectoryInfo di = new System.IO.DirectoryInfo("tempRepo/");
                BranchController.setAttributesNormal(di);
                di.Delete(true);
                return list;

                // Branches
                // special kind of reference
                



            }
        }
    }
}