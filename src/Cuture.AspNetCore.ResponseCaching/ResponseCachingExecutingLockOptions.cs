using System;

namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// 执行锁选项
    /// </summary>
    public class ResponseCachingExecutingLockOptions
    {
        #region Private 字段

        private TimeSpan _executingLockRecycleInterval = TimeSpan.FromMinutes(2);
        private int _maximumExecutingLockPooled = short.MaxValue >> 2;
        private int _maximumSemaphorePooled = short.MaxValue >> 2;
        private int _minimumExecutingLockPooled = (short.MaxValue >> 4) / 3;
        private int _minimumSemaphoreRetained = short.MaxValue >> 4;
        private TimeSpan _semaphoreRecycleInterval = TimeSpan.FromMinutes(4);

        #endregion Private 字段

        #region Public 属性

        /// <summary>
        /// 执行锁的回收间隔
        /// </summary>
        public TimeSpan ExecutingLockRecycleInterval
        {
            get => _executingLockRecycleInterval;
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(nameof(ExecutingLockRecycleInterval));
                }
                _executingLockRecycleInterval = value;
            }
        }

        /// <summary>
        /// 最大执行锁池大小
        /// </summary>
        public int MaximumExecutingLockPooled
        {
            get => _maximumExecutingLockPooled;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(MaximumExecutingLockPooled));
                }
                if (value < MinimumExecutingLockRetained)
                {
                    throw new ArgumentException($"cannot be less than {nameof(MinimumExecutingLockRetained)}", nameof(MaximumExecutingLockPooled));
                }
                _maximumExecutingLockPooled = value;
            }
        }

        /// <summary>
        /// 信号池的最大大小
        /// </summary>
        public int MaximumSemaphorePooled
        {
            get => _maximumSemaphorePooled;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(MaximumSemaphorePooled));
                }
                if (MinimumSemaphoreRetained > value)
                {
                    throw new ArgumentException($"cannot be less than {nameof(MinimumSemaphoreRetained)}", nameof(MaximumSemaphorePooled));
                }
                _maximumSemaphorePooled = value;
            }
        }

        /// <summary>
        /// 最小执行锁池大小
        /// </summary>
        public int MinimumExecutingLockRetained
        {
            get => _minimumExecutingLockPooled;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(MinimumExecutingLockRetained));
                }
                if (MaximumExecutingLockPooled < value)
                {
                    throw new ArgumentException($"cannot be large than {nameof(MaximumExecutingLockPooled)}", nameof(MinimumExecutingLockRetained));
                }
                _minimumExecutingLockPooled = value;
            }
        }

        /// <summary>
        /// 信号池的最小保留大小
        /// </summary>
        public int MinimumSemaphoreRetained
        {
            get => _minimumSemaphoreRetained;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(MinimumSemaphoreRetained));
                }
                if (MaximumSemaphorePooled < value)
                {
                    throw new ArgumentException($"cannot be large than {nameof(MaximumSemaphorePooled)}", nameof(MinimumSemaphoreRetained));
                }
                _minimumSemaphoreRetained = value;
            }
        }

        /// <summary>
        /// 信号池的回收间隔
        /// </summary>
        public TimeSpan SemaphoreRecycleInterval
        {
            get => _semaphoreRecycleInterval;
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(nameof(SemaphoreRecycleInterval));
                }
                _semaphoreRecycleInterval = value;
            }
        }

        #endregion Public 属性
    }
}