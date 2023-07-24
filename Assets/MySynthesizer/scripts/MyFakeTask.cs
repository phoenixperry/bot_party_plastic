//#define DEBUG_TASK

using System;
using System.Collections.Generic;

#if !WINDOWS_UWP
namespace MySpace.Tasks
{
    public class Task
    {
        private class Worker
        {
            private const int maxThreads = 3;
            private const int initialJobs = 1;
            private volatile bool exit;
            private readonly System.Threading.AutoResetEvent jobEvent;
            private readonly LinkedList<Task> tasks;
            private readonly LinkedList<Task> nodes;
            public Worker()
            {
                //UnityEngine.Debug.Log("worker start");
                exit = false;
                tasks = new LinkedList<Task>();
                nodes = new LinkedList<Task>();
                for (int i = 0; i < initialJobs; i++)
                {
                    Task empty = null;
                    nodes.AddLast(empty);
                }
#if DEBUG_TASK
                    jobEvent = null;
#else
                jobEvent = new System.Threading.AutoResetEvent(false);
                for (int i = 0; i < maxThreads; i++)
                {
                    System.Threading.Thread thread = new System.Threading.Thread(threadMain);
                    thread.Priority = System.Threading.ThreadPriority.Normal;
                    thread.IsBackground = false;
                    thread.Start();
                }
#endif
            }
            public void Terminate()
            {
                exit = true;
            }

            private void threadMain()
            {
                while (!exit)
                {
                    jobEvent.WaitOne(200);
                    while (tasks.Count != 0)
                    {
                        processJob();
                    }
                }
            }
            private void processJob()
            {
                Task task = null;
                lock (tasks)
                {
                    if (tasks.Count != 0)
                    {
                        LinkedListNode<Task> node = tasks.First;
                        tasks.Remove(node);
                        task = node.Value;
                        node.Value = null;
                        nodes.AddLast(node);
                    }
                    if (tasks.Count != 0)
                    {
                        jobEvent.Set();
                    }
                }
                if ((task != null) && !exit)
                {
                    try
                    {
                        task.state = TaskState.Running;
                        task.job.Invoke();
                        task.SetResult(TaskState.RanToCompletion);
                    }
                    catch (System.Exception e)
                    {
                        task.exception = e;
                        task.SetResult(TaskState.Faulted);
                        if (e is System.Threading.ThreadAbortException)
                        {
                        }
                        else
                        {
                            UnityEngine.Debug.LogError(e);
                        }
                    }
                }
            }
            public void Delegate(Task task)
            {
                task.state = TaskState.WaitingToRun;
#if DEBUG_TASK
                    lock (tasks)
                    {
                        try
                        {
                            task.state = TaskState.Running;
                            task.job.Invoke();
                            task.state = TaskState.RanToCompletion;
                        }
                        catch (System.Exception e)
                        {
                            UnityEngine.Debug.LogError(e);
                            task.exception = e;
                            task.state = TaskState.Faulted;
                        }
                    }
#else
                lock (tasks)
                {
                    LinkedListNode<Task> node = nodes.First;
                    if (node == null)
                    {
                        node = new LinkedListNode<Task>(task);
                    }
                    else
                    {
                        nodes.Remove(node);
                        node.Value = task;
                    }
                    tasks.AddLast(node);
                    jobEvent.Set();
                }
#endif
            }
        }

        public enum TaskState
        {
            Canceled,
            Created,
            Faulted,
            RanToCompletion,
            Running,
            WaitingForActivation,
            WaitingForChildrenToComplete,
            WaitingToRun,
        }
        private volatile TaskState state;
        private Exception exception;
        private Action job;
        private System.Threading.ManualResetEvent complete;

        private static Worker worker = new Worker();

        public Task(Action job)
        {
            //UnityEngine.Debug.Log("Task()");
            state = TaskState.Created;
            this.job = job;
        }
        private void SetResult(TaskState state)
        {
            lock (job)
            {
                this.state = state;
                if (complete != null)
                {
                    complete.Set();
                }
            }
        }

        public TaskState Status
        {
            get
            {
                return state;
            }
        }
        public Exception Exception
        {
            get
            {
                return exception;
            }
        }
        public bool IsCompleted
        {
            get
            {
                TaskState st = state;
                return (st == TaskState.Canceled) || (st == TaskState.Faulted) || (st == TaskState.RanToCompletion);
            }
        }
        public static Task Run(Action action)
        {
            Task t = new Task(action);
            t.Start();
            return t;
        }
        public void Start()
        {
            if (state != TaskState.Created)
            {
                throw new System.InvalidOperationException();
            }
            state = TaskState.WaitingForActivation;
            worker.Delegate(this);
        }
        public void Wait()
        {
            if (IsCompleted)
            {
                return;
            }
            lock (job)
            {
                if (IsCompleted)
                {
                    return;
                }
                complete = new System.Threading.ManualResetEvent(false);
            }
            complete.WaitOne();
            complete.Close();
            complete = null;
        }
    }
}
#endif
