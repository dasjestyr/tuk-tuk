using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Provausio.TukTuk.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class TukBusTests
    {
        private TukBus _bus;

        [TestInitialize]
        public void Init()
        {
            _bus = new TukBus();
            _bus.Clear();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Subscribe_NullHandler_Throws()
        {
            // arrange

            // act
            _bus.Subscribe<FakeEvent1>(null);

            // assert
            Assert.Fail();
        }

        [TestMethod]
        public void Subscribe_ReturnsHandle()
        {
            // arrange
            Action<IEvent> handler = args => { };

            // act
            var handle = _bus.Subscribe<FakeEvent1>(handler);

            // assert
            Assert.IsNotNull(handle);
        }

        [TestMethod]
        public void Subscribe_DuplicateHandler_ReturnsSameHandle()
        {
            // arrange
            Action<IEvent> handler = args => { };

            // act
            var handle1 = _bus.Subscribe<FakeEvent1>(handler);
            var handle2 = _bus.Subscribe<FakeEvent1>(handler);

            // assert
            Assert.AreEqual(handle1, handle2);
        }

        [TestMethod]
        [ExpectedException(typeof (InvalidOperationException))]
        public void Subscribe_Disposed_Throws()
        {
            // arrange
            var bus = new TukBus();
            Action<IEvent> handler = args => { };

            // act
            using (bus) { }
            bus.Subscribe<FakeEvent1>(handler);
        }

        [TestMethod]
        public void Publish_Handler_IsCalled()
        {
            // arrange
            const string expectedValue = "Hit";
            var actualValue = string.Empty;
            Action<IEvent> handler = args => { actualValue = expectedValue; };
            _bus.Subscribe<FakeEvent1>(handler);

            // act
            _bus.Publish(new FakeEvent1());
            Thread.Sleep(150);

            // assert
            Assert.AreEqual(expectedValue, actualValue);
        }

        [TestMethod]
        public void Publish_TwoHandlers_BothCalled()
        {
            // arrange
            const string expected1 = "One";
            const string expected2 = "Two";
            var actual1 = string.Empty;
            var actual2 = string.Empty;
            Action<IEvent> handler1 = args => { actual1 = expected1; };
            Action<IEvent> handler2 = args => { actual2 = expected2; };
            _bus.Subscribe<FakeEvent1>(handler1);
            _bus.Subscribe<FakeEvent1>(handler2);

            // act
            _bus.Publish(new FakeEvent1());
            Thread.Sleep(150);

            // assert
            Assert.AreEqual(expected1, actual1, "First handler should have been invoked");
            Assert.AreEqual(expected2, actual2, "Handler 2 should have been invoked");
        }

        [TestMethod]
        public void Publish_TwoHandlers_OnlyFirstIsCalled()
        {
            // arrange
            const string expected1 = "One";
            const string expected2 = "Two";
            var actual1 = string.Empty;
            var actual2 = string.Empty;
            Action<IEvent> handler1 = args => { actual1 = expected1; };
            Action<IEvent> handler2 = args => { actual2 = expected2; };
            _bus.Subscribe<FakeEvent1>(handler1);
            _bus.Subscribe<FakeEvent2>(handler2);

            // act
            _bus.Publish(new FakeEvent1());
            Thread.Sleep(150);

            // assert
            Assert.AreEqual(expected1, actual1, "First handler should have been invoked");
            Assert.AreNotEqual(expected2, actual2, "Handler 2 should not have been invoked");
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Publish_Disposed_Throws()
        {
            // arrange
            var bus = new TukBus();

            // act
            using (bus) { }
            bus.Publish(new FakeEvent1());

            // assert
            Assert.Fail();
        }

        [TestMethod]
        public void Dispose_Disposes()
        {
            // arrange
            var bus = new TukBus();
            const string expected1 = "One";
            const string expected2 = "Two";
            var actual1 = string.Empty;
            var actual2 = string.Empty;
            Action<IEvent> handler1 = args => { actual1 = expected1; };
            Action<IEvent> handler2 = args => { actual2 = expected2; };
            bus.Subscribe<FakeEvent1>(handler1);
            bus.Subscribe<FakeEvent2>(handler2);

            // act
            using (bus) { }

            // assert
            Assert.IsTrue(bus.IsDisposed);
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Dispose_AlreadyDisposed_Throws()
        {
            // arrange
            var bus = new TukBus();

            // act
            using (bus) { }
            using (bus) { }

            // assert
            Assert.Fail();
        }

        private class FakeEvent1 : IEvent { }

        private class FakeEvent2 : IEvent { }
    }
}
