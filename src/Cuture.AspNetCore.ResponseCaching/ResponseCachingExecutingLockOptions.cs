using System;
using System.Diagnostics;

using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Cuture.AspNetCore.ResponseCaching
{
    /// <summary>
    /// 执行锁选项
    /// </summary>
    public class ResponseCachingExecutingLockOptions : IOptions<ResponseCachingExecutingLockOptions>
    {
        #region Private 字段

        #region Semaphore

        private int _maximumSemaphorePooled = short.MaxValue >> 2;
        private int _minimumSemaphoreRetained = short.MaxValue >> 4;
        private TimeSpan _semaphoreRecycleInterval = TimeSpan.FromMinutes(4);

        #endregion Semaphore

        #region LockState

        private TimeSpan _lockStateRecycleInterval = TimeSpan.FromMinutes(2);
        private int _maximumLockStatePooled = short.MaxValue >> 2;
        private int _minimumLockStateRetained = (short.MaxValue >> 4) / 3;

        #endregion LockState

        private readonly ILogger? _logger;
        private bool _checked = false;

        #endregion Private 字段

        #region Public 属性

        #region Semaphore

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
                _maximumSemaphorePooled = value;
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

        #endregion Semaphore

        #region LockState

        /// <summary>
        /// 锁定内容的回收间隔
        /// </summary>
        public TimeSpan LockStateRecycleInterval
        {
            get => _lockStateRecycleInterval;
            set
            {
                if (value <= TimeSpan.Zero)
                {
                    throw new ArgumentOutOfRangeException(nameof(LockStateRecycleInterval));
                }
                _lockStateRecycleInterval = value;
            }
        }

        /// <summary>
        /// 最大锁定内容池大小
        /// </summary>
        public int MaximumLockStatePooled
        {
            get => _maximumLockStatePooled;
            set
            {
                if (value <= 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(MaximumLockStatePooled));
                }
                _maximumLockStatePooled = value;
            }
        }

        /// <summary>
        /// 最小锁定内容池保留大小
        /// </summary>
        public int MinimumLockStateRetained
        {
            get => _minimumLockStateRetained;
            set
            {
                if (value < 0)
                {
                    throw new ArgumentOutOfRangeException(nameof(MinimumLockStateRetained));
                }
                _minimumLockStateRetained = value;
            }
        }

        #endregion LockState

        /// <inheritdoc/>
        public ResponseCachingExecutingLockOptions Value => this;

        #endregion Public 属性

        #region Public 构造函数

        /// <inheritdoc cref="ResponseCachingExecutingLockOptions"/>
        public ResponseCachingExecutingLockOptions()
        {
        }

        /// <inheritdoc cref="ResponseCachingExecutingLockOptions"/>
        public ResponseCachingExecutingLockOptions(ILogger<ResponseCachingExecutingLockOptions> logger)
        {
            _logger = logger;
        }

        #endregion Public 构造函数

        #region Internal 方法

        internal void CheckOptions()
        {
            Debug.WriteLine("ResponseCachingExecutingLockOptions.CheckOptions");
            if (_checked)
            {
                return;
            }
            if (MaximumSemaphorePooled < MinimumSemaphoreRetained)
            {
                throw new ArgumentException($"cannot be less than {MinimumSemaphoreRetained}", nameof(MaximumSemaphorePooled));
            }

            if (MaximumLockStatePooled < MinimumLockStateRetained)
            {
                throw new ArgumentException($"cannot be less than {MinimumLockStateRetained}", nameof(MaximumLockStatePooled));
            }

            if (_logger is not null)
            {
                if (MaximumSemaphorePooled < 512)
                {
                    _logger.LogWarning($"the value of {MaximumSemaphorePooled} maybe not big enough.");
                }

                if (MaximumLockStatePooled > MaximumSemaphorePooled)
                {
                    _logger.LogWarning($"a value for {nameof(MaximumLockStatePooled)} greater than {nameof(MaximumSemaphorePooled)} is worthless.");
                }
            }
            _checked = true;
        }

        #endregion Internal 方法
    }
}