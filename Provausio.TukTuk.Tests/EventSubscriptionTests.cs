using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace Provausio.TukTuk.Tests
{
    [ExcludeFromCodeCoverage]
    [TestClass]
    public class EventSubscriptionTests
    {
        private Mock<IUnsubscribeSource> _unsub;

        [TestInitialize]
        public void Init()
        {
            _unsub = new Mock<IUnsubscribeSource>();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullHandler_Throws()
        {
            // arrange
            
            // act
            var sub = new EventSubscription(typeof (Event), null, _unsub.Object);

            // assert
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentNullException))]
        public void Ctor_NullEventType_Throws()
        {
            // arrange

            // act
            var sub = new EventSubscription(null, a => { }, _unsub.Object);

            // assert
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void Ctor_NullUnsub_Throws()
        {
            // arrange

            // act
            var sub = new EventSubscription(typeof(Event), a => { }, null);

            // assert
            Assert.Fail();
        }

        [TestMethod]
        [ExpectedException(typeof (ArgumentNullException))]
        public void Handle_NullEvent_Throws()
        {
            // arrange
            var sub = new EventSubscription(typeof(Event), a => { }, _unsub.Object);

            // act
            sub.Handle(null);

            // assert
            Assert.Fail();
        }

        [TestMethod]
        public void Equals_DifferentObject_IsFalse()
        {
            // arrange
            var sub = new EventSubscription(typeof(Event), a => { }, _unsub.Object);
            var other = "Hello";

            // act
            
            // assert
            Assert.AreNotEqual(sub, other);
        }

        [TestMethod]
        public void Equals_SameObject_IsTrue()
        {
            // arrange
            var sub = new EventSubscription(typeof(Event), a => { }, _unsub.Object);

            // act

            // assert
            Assert.AreEqual(sub, sub);
        }

        [TestMethod]
        public void Equals_DifferentValue_IsFalse()
        {
            // arrange
            var sub1 = new EventSubscription(typeof(Event), a => { }, _unsub.Object);
            var sub2 = new EventSubscription(typeof(Event), a => { }, _unsub.Object);

            // act
            
            // assert
            Assert.AreNotEqual(sub1, sub2);
        }

        [TestMethod]
        public void GetHashCode_SameValue_SameCode()
        {
            // arrange
            var sub = new EventSubscription(typeof(Event), a => { }, _unsub.Object);

            // act

            // assert
            Assert.AreEqual(sub.GetHashCode(), sub.GetHashCode());
        }

        [TestMethod]
        public void GetHashCode_DifferentValue_DifferentCode()
        {
            // arrange
            var sub1 = new EventSubscription(typeof(Event), a => { }, _unsub.Object);
            var sub2 = new EventSubscription(typeof(Event), a => { }, _unsub.Object);

            // act

            // assert
            Assert.AreNotEqual(sub1.GetHashCode(), sub2.GetHashCode());
        }
    }
}
