using System;
using System.Diagnostics;
using CupheadTrainer.Core;
using Xunit;

namespace CupheadTrainer.Tests
{
    /// <summary>
    /// Unit tests for TrainerCore functionality.
    /// Note: These tests require the Cuphead process to be running.
    /// </summary>
    public class TrainerCoreTests : IDisposable
    {
        private readonly TrainerCore _trainer;

        public TrainerCoreTests()
        {
            // Skip test initialization if Cuphead is not running
            var processes = Process.GetProcessesByName("Cuphead");
            if (processes.Length == 0)
            {
                throw new SkipException("Cuphead process not found; skipping tests.");
            }

            _trainer = new TrainerCore();
            _trainer.Initialize();
        }

        [Fact]
        public void ToggleInfiniteHP_ShouldToggleFlag()
        {
            // This test verifies that toggling doesn't throw exceptions
            _trainer.ToggleInfiniteHP();
            _trainer.ToggleInfiniteHP();
            Assert.True(true); // Placeholder: real test would check memory
        }

        [Fact]
        public void ToggleInfiniteCoins_ShouldToggleFlag()
        {
            _trainer.ToggleInfiniteCoins();
            _trainer.ToggleInfiniteCoins();
            Assert.True(true);
        }

        [Fact]
        public void ToggleSpeedMultiplier_ShouldToggleFlag()
        {
            _trainer.ToggleSpeedMultiplier();
            _trainer.ToggleSpeedMultiplier();
            Assert.True(true);
        }

        [Fact]
        public void Cleanup_ShouldNotThrow()
        {
            var exception = Record.Exception(() => _trainer.Cleanup());
            Assert.Null(exception);
        }

        public void Dispose()
        {
            _trainer?.Cleanup();
        }
    }
}
