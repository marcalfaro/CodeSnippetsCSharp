using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CodeSnippetsCSharp
{
    internal class Program
    {
        static void Main(string[] args)
        {

            // Spawn subtasks to fetch data by chunks
            List<Task<DataTable>> taskList = new List<Task<DataTable>>();

            for (int i = 1; i <= totalPages; i++)
            {
                taskList.Add(Task.Run(async () =>
                {
                    DataTable tdt = null;
                    try
                    {
                        using (CancellationTokenSource cts = new CancellationTokenSource(TimeSpan.FromMinutes(2)))
                        {
                            tdt = await someTask();
                        }
                    }
                    catch (Exception ex)
                    {
                        Debug.Print(ex.Message);
                    }
                    return tdt;
                }));

                await Task.Delay(10);
            }

            // Consolidate all tasks. Wait for any task that compleres and process it.
            while (taskList.Count > 0)
            {
                Task<DataTable> completedTask = null;
                try
                {
                    completedTask = await Task.WhenAny(taskList.ToArray());
                    if (completedTask != null && completedTask.Result != null)
                    {
                        if (dtAll == null)
                            dtAll = completedTask.Result;
                        else
                            dtAll.Merge(completedTask.Result);
                    }
                }
                catch (Exception ex)
                {
                    Debug.Print(ex.Message);
                }
                finally
                {
                    if (completedTask != null)
                    {
                        taskList.Remove(completedTask);
                        completedTask.Dispose();
                        completedTask = null;
                    }
                }
                await Task.Delay(10);
            }

        }
    }
}
