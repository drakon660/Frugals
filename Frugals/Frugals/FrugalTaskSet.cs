using System.Runtime.CompilerServices;

namespace FrugalTasks
{
    /// <summary>
    /// A high-performance value type for efficiently awaiting tasks that optimizes for the single-task case.
    /// Uses ReadOnlyMemory for efficient storage and supports single-task optimization.
    /// </summary>
    public readonly struct FrugalTaskSet
    {
        // Single task optimization - avoid allocation when only one task
        private readonly Task _singleTask;
        
        // For multiple tasks, use ReadOnlyMemory for efficient storage
        private readonly ReadOnlyMemory<Task> _tasks;
        
        // Track count to avoid accessing _tasks when we have a single task
        private readonly int _count;

        /// <summary>
        /// Creates a new FrugalTaskSet from a single task.
        /// Optimized constructor that avoids array allocation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FrugalTaskSet(Task task)
        {
            _singleTask = task;
            _tasks = default;
            _count = task != null ? 1 : 0;
        }

        /// <summary>
        /// Creates a new FrugalTaskSet from an array of tasks.
        /// </summary>
        public FrugalTaskSet(Task[] tasks)
        {
            if (tasks == null || tasks.Length == 0)
            {
                _singleTask = null;
                _tasks = default;
                _count = 0;
            }
            else if (tasks.Length == 1)
            {
                _singleTask = tasks[0];
                _tasks = default;
                _count = 1;
            }
            else
            {
                _singleTask = null;
                _tasks = tasks;
                _count = tasks.Length;
            }
        }

        /// <summary>
        /// Creates a new FrugalTaskSet from a ReadOnlyMemory of tasks.
        /// </summary>
        public FrugalTaskSet(ReadOnlyMemory<Task> tasks)
        {
            if (tasks.IsEmpty)
            {
                _singleTask = null;
                _tasks = default;
                _count = 0;
            }
            else if (tasks.Length == 1)
            {
                _singleTask = tasks.Span[0];
                _tasks = default;
                _count = 1;
            }
            else
            {
                _singleTask = null;
                _tasks = tasks;
                _count = tasks.Length;
            }
        }

        /// <summary>
        /// Creates a new FrugalTaskSet from a ReadOnlySpan of tasks.
        /// </summary>
        public FrugalTaskSet(ReadOnlySpan<Task> tasks)
        {
            if (tasks.IsEmpty)
            {
                _singleTask = null;
                _tasks = default;
                _count = 0;
            }
            else if (tasks.Length == 1)
            {
                _singleTask = tasks[0];
                _tasks = default;
                _count = 1;
            }
            else
            {
                // Need to copy span to memory for storage
                var array = tasks.ToArray();
                _singleTask = null;
                _tasks = array;
                _count = array.Length;
            }
        }

        /// <summary>
        /// Creates a new FrugalTaskSet from a params array of tasks.
        /// </summary>
        public static FrugalTaskSet Create(params Task[] tasks)
        {
            return new FrugalTaskSet(tasks);
        }

        /// <summary>
        /// Number of tasks in the set.
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// Efficiently waits for all tasks to complete, optimizing for the single-task case.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task WaitAllAsync()
        {
            switch (_count)
            {
                case 0:
                    return Task.CompletedTask;
                case 1:
                    return _singleTask;
                default:
                    return Task.WhenAll(_tasks.Span);
            }
        }
    }

    /// <summary>
    /// A high-performance value type for efficiently awaiting generic tasks that optimizes for the single-task case.
    /// Uses ReadOnlyMemory for efficient storage and supports single-task optimization.
    /// </summary>
    public readonly struct FrugalTaskSet<T>
    {
        // Single task optimization - avoid allocation when only one task
        private readonly Task<T> _singleTask;
        
        // For multiple tasks, use ReadOnlyMemory for efficient storage
        private readonly ReadOnlyMemory<Task<T>> _tasks;
        
        // Track count to avoid accessing _tasks when we have a single task
        private readonly int _count;

        /// <summary>
        /// Creates a new FrugalTaskSet from a single task.
        /// Optimized constructor that avoids array allocation.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public FrugalTaskSet(Task<T> task)
        {
            _singleTask = task;
            _tasks = default;
            _count = task != null ? 1 : 0;
        }

        /// <summary>
        /// Creates a new FrugalTaskSet from an array of tasks.
        /// </summary>
        public FrugalTaskSet(Task<T>[] tasks)
        {
            if (tasks.Length == 0)
            {
                _singleTask = null;
                _tasks = default;
                _count = 0;
            }
            else if (tasks.Length == 1)
            {
                _singleTask = tasks[0];
                _tasks = default;
                _count = 1;
            }
            else
            {
                _singleTask = null;
                _tasks = tasks;
                _count = tasks.Length;
            }
        }

        /// <summary>
        /// Creates a new FrugalTaskSet from a ReadOnlyMemory of tasks.
        /// </summary>
        public FrugalTaskSet(ReadOnlyMemory<Task<T>> tasks)
        {
            if (tasks.IsEmpty)
            {
                _singleTask = null;
                _tasks = default;
                _count = 0;
            }
            else if (tasks.Length == 1)
            {
                _singleTask = tasks.Span[0];
                _tasks = default;
                _count = 1;
            }
            else
            {
                _singleTask = null;
                _tasks = tasks;
                _count = tasks.Length;
            }
        }

        /// <summary>
        /// Creates a new FrugalTaskSet from a ReadOnlySpan of tasks.
        /// </summary>
        public FrugalTaskSet(ReadOnlySpan<Task<T>> tasks)
        {
            if (tasks.IsEmpty)
            {
                _singleTask = null;
                _tasks = default;
                _count = 0;
            }
            else if (tasks.Length == 1)
            {
                _singleTask = tasks[0];
                _tasks = default;
                _count = 1;
            }
            else
            {
                // Need to copy span to memory for storage
                var array = tasks.ToArray();
                _singleTask = null;
                _tasks = array;
                _count = array.Length;
            }
        }

        /// <summary>
        /// Creates a new FrugalTaskSet from a params array of tasks.
        /// </summary>
        public static FrugalTaskSet<T> Create(params Task<T>[] tasks)
        {
            return new FrugalTaskSet<T>(tasks);
        }

        /// <summary>
        /// Number of tasks in the set.
        /// </summary>
        public int Count => _count;

        /// <summary>
        /// Efficiently waits for all tasks to complete, optimizing for the single-task case.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Task WaitAllAsync()
        {
            switch (_count)
            {
                case 0:
                    return Task.CompletedTask;
                case 1:
                    return _singleTask;
                default:
                    return Task.WhenAll(_tasks.Span);
            }
        }

        /// <summary>
        /// Efficiently waits for all tasks to complete and returns their results,
        /// optimizing for the single-task case.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public async Task<T[]> WaitAllAndGetResultsAsync()
        {
            switch (_count)
            {
                case 0:
                    return await Task.FromResult(Array.Empty<T>());
                case 1:
                    return [await _singleTask];
                default:
                    return await Task.WhenAll(_tasks.Span);
            }
        }
    }

    /// <summary>
    /// Extension methods for creating FrugalTaskSets from various sources
    /// </summary>
    public static class TaskExtensions
    {
        /// <summary>
        /// Creates a FrugalTaskSet from a single task
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FrugalTaskSet ToFrugalSet(this Task task)
        {
            return new FrugalTaskSet(task);
        }

        /// <summary>
        /// Creates a FrugalTaskSet from an array of tasks
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FrugalTaskSet ToFrugalSet(this Task[] tasks)
        {
            return new FrugalTaskSet(tasks);
        }

        /// <summary>
        /// Creates a FrugalTaskSet from a ReadOnlyMemory of tasks
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FrugalTaskSet ToFrugalSet(this ReadOnlyMemory<Task> tasks)
        {
            return new FrugalTaskSet(tasks);
        }

        /// <summary>
        /// Creates a FrugalTaskSet from a ReadOnlySpan of tasks
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FrugalTaskSet ToFrugalSet(this ReadOnlySpan<Task> tasks)
        {
            return new FrugalTaskSet(tasks);
        }

        /// <summary>
        /// Creates a FrugalTaskSet from a list of tasks
        /// </summary>
        public static FrugalTaskSet ToFrugalSet(this List<Task> tasks)
        {
            return tasks.Count switch
            {
                0 => default,
                1 => new FrugalTaskSet(tasks[0]),
                _ => new FrugalTaskSet(tasks.ToArray())
            };
        }
        
        /// <summary>
        /// Creates a FrugalTaskSet from a single task
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FrugalTaskSet<T> ToFrugalSet<T>(this Task<T> task)
        {
            return new FrugalTaskSet<T>(task);
        }

        /// <summary>
        /// Creates a FrugalTaskSet from an array of tasks
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FrugalTaskSet<T> ToFrugalSet<T>(this Task<T>[] tasks)
        {
            return new FrugalTaskSet<T>(tasks);
        }

        /// <summary>
        /// Creates a FrugalTaskSet from a ReadOnlyMemory of tasks
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FrugalTaskSet<T> ToFrugalSet<T>(this ReadOnlyMemory<Task<T>> tasks)
        {
            return new FrugalTaskSet<T>(tasks);
        }

        /// <summary>
        /// Creates a FrugalTaskSet from a ReadOnlySpan of tasks
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static FrugalTaskSet<T> ToFrugalSet<T>(this ReadOnlySpan<Task<T>> tasks)
        {
            return new FrugalTaskSet<T>(tasks);
        }

        /// <summary>
        /// Creates a FrugalTaskSet from a list of tasks
        /// </summary>
        public static FrugalTaskSet<T> ToFrugalSet<T>(this List<Task<T>> tasks)
        {
            return tasks.Count switch
            {
                0 => default,
                1 => new FrugalTaskSet<T>(tasks[0]),
                _ => new FrugalTaskSet<T>(tasks.ToArray())
            };
        }
    }
}