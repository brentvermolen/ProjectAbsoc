using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BL.Jobs
{
    public class CleanActors : IJob
    {
        private ActeurManager ActeurMng = new ActeurManager();

        Task IJob.Execute(IJobExecutionContext context)
        {
            var acteurs = ActeurMng.ReadActeurs();

            for (int i = 0; i < acteurs.Count; i++)
            {
                var acteur = acteurs[i];

                if (acteur.Series.Count == 0 && acteur.Films.Count == 0)
                {
                    ActeurMng.RemoveActeur(acteur.ID);
                }
            }


            return Task.FromResult(0);
        }
    }
}
