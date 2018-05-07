using System;
using UnityEngine;
using System.Collections;

namespace Helpers
{

    /// ----------------------------------------------------------------------------
    /// IEnumerator MyAwesomeTask()
    /// {
    ///     while(true) {
    ///         Debug.Log("Logcat iz in ur consolez, spammin u wif messagez.");
    ///         yield return null;
    ////    }
    /// }
    ///
    /// IEnumerator TaskKiller(float delay, Task t)
    /// {
    ///     yield return new WaitForSeconds(delay);
    ///     t.Stop();
    /// }
    ///
    /// void SomeCodeThatCouldBeAnywhereInTheUniverse()
    /// {
    ///     Task spam = new Task(MyAwesomeTask());
    ///     new Task(TaskKiller(5, spam));
    /// }
    /// ----------------------------------------------------------------------------
    ///
    /// When SomeCodeThatCouldBeAnywhereInTheUniverse is called, the debug console
    /// will be spammed with annoying messages for 5 seconds.
    ///
    /// Simple, really.  There is no need to initialize or even refer to TaskManager.
    /// When the first Task is created in an application, a "TaskManager" GameObject
    /// will automatically be added to the scene root with the TaskManager component
    /// attached.  This component will be responsible for dispatching all coroutines
    /// behind the scenes.
    ///
    /// Task also provides an event that is triggered when the coroutine exits.
    /// 
    /// A Task object represents a coroutine.  Tasks can be started, paused, and stopped.
    /// It is an error to attempt to start a task that has been stopped or which has
    /// naturally terminated.
    /// 
    /// 
    public class Task
    {
        /// Returns true if and only if the coroutine is running.  Paused tasks
        /// are considered to be running.
        public bool Running
        {
            get
            {
                return _taskState.Running;
            }
        }

        /// Returns true if and only if the coroutine is currently paused.
        public bool Paused
        {
            get
            {
                return _taskState.Paused;
            }
        }

        /// Delegate for termination subscribers.  manual is true if and only if
        /// the coroutine was stopped with an explicit call to Stop().
        public delegate void FinishedHandler(bool manual);

        /// Termination event.  Triggered when the coroutine completes execution.
        private event FinishedHandler _finished;
        public event FinishedHandler Finished
        {
            add
            {
                _finished += value;
            }
            remove
            {
                _finished -= value;
            }
        }



        /// Creates a new Task object for the given coroutine.
        ///
        /// If autoStart is true (default) the task is automatically started
        /// upon construction.
        public Task(IEnumerator c, bool autoStart = true)
        {
            _taskState = TaskManager.CreateTask(c);
            _taskState.Finished += _taskStateFinished;
            if (autoStart)
                Start();
        }

        private Func<IEnumerator> _c;

        /// <summary>
        /// Restartable Task
        /// </summary>
        /// <param name="c"></param>
        /// <param name="autoStart"></param>
        public Task(Func<IEnumerator> c, bool autoStart = true)
        {
            _c = c;
            _taskState = TaskManager.CreateTask(_c());
            _taskState.Finished += _taskStateFinished;
            if (autoStart)
                Start();
        }

        /// Begins execution of the coroutine
        public void Start()
        {
            _taskState.Start();
        }

        /// Discontinues execution of the coroutine at its next yield.
        public void Stop()
        {
            _taskState.Stop();
        }

        public void Pause()
        {
            _taskState.Pause();
        }

        public void Unpause()
        {
            _taskState.Unpause();
        }

        public void Restart()
        {
            _taskState.Stop();
            _taskState = new TaskManager.TaskState(_c());
            _taskState.Start();
        }

        private void _taskStateFinished(bool manual)
        {
            FinishedHandler handler = _finished;
            if (handler != null)
                handler(manual);
        }

        private TaskManager.TaskState _taskState;
    }

    internal class TaskManager : MonoBehaviour
    {
        public class TaskState
        {
            public bool Running
            {
                get
                {
                    return running;
                }
            }

            public bool Paused
            {
                get
                {
                    return paused;
                }
            }

            public delegate void FinishedHandler(bool manual);

            public event FinishedHandler Finished;

            private IEnumerator _coroutine;
            private bool running;
            private bool paused;
            private bool stopped;

            public TaskState(IEnumerator c)
            {
                _coroutine = c;
            }

            public void Pause()
            {
                paused = true;
            }

            public void Unpause()
            {
                paused = false;
            }

            public void Start()
            {
                running = true;
                singleton.StartCoroutine(CallWrapper());
            }

            public void Stop()
            {
                stopped = true;
                running = false;
            }

            private IEnumerator CallWrapper()
            {
                //Debug.Log("CallWrapper");
                yield return null;

                UnityEngine.Profiling.Profiler.BeginSample("CallWrapper 1: " + _coroutine.ToString());
                IEnumerator e = _coroutine;
                UnityEngine.Profiling.Profiler.EndSample();
                while (running)
                {
                    if (paused)
                        yield return null;
                    else
                    {
                        UnityEngine.Profiling.Profiler.BeginSample("CallWrapper 2: " + e);
                        bool flag = e.MoveNext();
                        UnityEngine.Profiling.Profiler.EndSample();

                        if (e != null && flag)
                        {
                            yield return e.Current;
                        }
                        else
                        {
                            running = false;
                        }
                    }
                }
                UnityEngine.Profiling.Profiler.BeginSample("CallWrapper 3: " + e.ToString());
                FinishedHandler handler = Finished;
                if (handler != null)
                    handler(stopped);
                UnityEngine.Profiling.Profiler.EndSample();
            }
        }

        private static TaskManager singleton;

        public static TaskState CreateTask(IEnumerator coroutine)
        {
            if (singleton == null)
            {
                var go = new GameObject("TaskManager");
                singleton = go.AddComponent<TaskManager>();
            }
            return new TaskState(coroutine);
        }
    }
}
