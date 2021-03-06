﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using SchedulerHandout;
using SchedulerHandout.Exceptions;

namespace Scheduler.Test.Unit
{
    [TestFixture]
    public class SchedulerUnitTest
    {
        private IScheduler _uut;
        [SetUp]
        public void Setup()
        {
            _uut = new SchedulerHandout.Scheduler();
        }

        #region Spawn

        [Test]
        public void Spawn_SameName_ThrowException()
        {
            _uut.Spawn("Alex", SchedulerHandout.Scheduler.Priority.High);
            Assert.Throws<ThreadAlreadyExistException>(
                () => _uut.Spawn("Alex", SchedulerHandout.Scheduler.Priority.High));
        }

        #endregion
        
        #region Schedule

        [Test]
        public void Schedule_1HighPrioThread_ActiveThread()
        {
            _uut.Spawn("Alex", SchedulerHandout.Scheduler.Priority.High);
            _uut.Schedule();
            Assert.That(_uut.ActiveThread, Is.EqualTo("Alex"));
        }

        [Test]
        public void Schedule_1MedPrioThread_ActiveThread()
        {
            _uut.Spawn("Nanna", SchedulerHandout.Scheduler.Priority.Med);
            _uut.Schedule();
            Assert.That(_uut.ActiveThread, Is.EqualTo("Nanna"));
        }

        [Test]
        public void Schedule_1LowPrioThread_ActiveThread()
        {
            _uut.Spawn("Jeba", SchedulerHandout.Scheduler.Priority.Low);
            _uut.Schedule();
            Assert.That(_uut.ActiveThread, Is.EqualTo("Jeba"));
        }

        [Test]
        public void Schedule_3PriorityThreads_ActiveThread()
        {
            _uut.Spawn("Alex", SchedulerHandout.Scheduler.Priority.Low);
            _uut.Spawn("Nanna", SchedulerHandout.Scheduler.Priority.Med);
            _uut.Spawn("Jeba", SchedulerHandout.Scheduler.Priority.High);
            _uut.Schedule();
            Assert.That(_uut.ActiveThread, Is.EqualTo("Jeba"));
        }

        [Test]
        public void Schedule_EmptyThreadLists_ThrowNoThreadsActiveException()
        {
            Assert.Throws<NoThreadsActiveException>(() => _uut.Schedule());
        }

        #endregion
        
        #region Kill

        [Test]
        public void Kill_NoThreadsExist_ThrowThreadNotFoundExceoption()
        {
            Assert.Throws<NoThreadsActiveException>(() => _uut.Kill("android"));
        }

        [Test]
        public void Kill_SpawnAndKill_TestOnScheduleThrowSHNoThreadsToScheduleException()
        {
            _uut.Spawn("android", SchedulerHandout.Scheduler.Priority.Low);
            _uut.Kill("android");
            var ex = Assert.Throws<NoThreadsActiveException>(() => _uut.Schedule());
            Assert.That(ex.Message, Is.EqualTo("No active threads!"));
        }

        [Test]
        public void Kill_NoThreadFound_ThrowsSHKillException()
        {
            _uut.Spawn("Apple", SchedulerHandout.Scheduler.Priority.High);
            var ex = Assert.Throws<ThreadNotFoundExceoption>(() => _uut.Kill("Android"));
            Assert.That(ex.Message, Is.EqualTo("No active threads!"));
        }

        [Test]
        public void Kill_2ThreadsFound_Schedule()
        {
            _uut.Spawn("Apple", SchedulerHandout.Scheduler.Priority.Med);
            _uut.Spawn("Magic", SchedulerHandout.Scheduler.Priority.High);
            _uut.Schedule();
            _uut.Kill("Magic");
            Assert.That(_uut.NThreads, Is.EqualTo(0));
        }
        [Test]
        public void Kill_1ThreadFoundSameName_Schedule()
        {
            _uut.Spawn("Magic", SchedulerHandout.Scheduler.Priority.High);
            _uut.Schedule();
            _uut.Kill("Magic");
            Assert.That(_uut.NThreads, Is.EqualTo(0));
        }

        #endregion

        #region SetPriority

        [Test]
        public void SetPriority_ThreadNotFound_ThrowException()
        {
            Assert.Throws<ThreadNotFoundExceoption>(
                () => _uut.SetPriority("Alex", SchedulerHandout.Scheduler.Priority.Low));
        }

        [Test]
        public void SetPriority_FromHighToLow_NoException()
        {
            _uut.Spawn("Alex", SchedulerHandout.Scheduler.Priority.Med);
            _uut.Spawn("Jeba", SchedulerHandout.Scheduler.Priority.Low);
            _uut.SetPriority("Jeba", SchedulerHandout.Scheduler.Priority.High);
            _uut.Schedule();
            Assert.That(_uut.ActiveThread, Is.EqualTo("Jeba"));
        }
        #endregion

        #region GetPriority

        [Test]
        public void GetPrio_threadAlex_returnHigh()
        {
            _uut.Spawn("Alex", SchedulerHandout.Scheduler.Priority.High);
            Assert.That(_uut.GetPriority("Alex"), Is.EqualTo(SchedulerHandout.Scheduler.Priority.High));
        }

        [Test]
        public void GetPrio_threadAlex_returnMed()
        {
            _uut.Spawn("Alex", SchedulerHandout.Scheduler.Priority.Med);
            Assert.That(_uut.GetPriority("Alex"), Is.EqualTo(SchedulerHandout.Scheduler.Priority.Med));
        }

        [Test]
        public void GetPrio_threadAlex_returnLow()
        {
            _uut.Spawn("Alex", SchedulerHandout.Scheduler.Priority.Low);
            Assert.That(_uut.GetPriority("Alex"), Is.EqualTo(SchedulerHandout.Scheduler.Priority.Low));
        }

        [Test]
        public void GetPrio_threadNanna_ThrowException()
        {
            Assert.Throws<ThreadNotFoundExceoption>(() => _uut.GetPriority("Nanna"));
        }

        #endregion

        #region ReName

        [Test]
        public void ReName_AlextoNanna_SpawnNewNanna_ThrowAlreadyExist()
        {
            _uut.Spawn("Alex", SchedulerHandout.Scheduler.Priority.High);
            _uut.Rename("Alex", "Nanna");
            Assert.Throws<ThreadAlreadyExistException>(
                () => _uut.Spawn("Nanna", SchedulerHandout.Scheduler.Priority.High));
        }

        [Test]
        public void ReName_AlextoNanna_ThrowNameDoesNotExist()
        {
            Assert.Throws<ThreadNotFoundExceoption>(
                () => _uut.Rename("Alex", "Nanna"));
        }

        #endregion

        #region GetNThreads

        [Test]
        public void GetNThreads_Spawn3High_return3()
        {
            _uut.Spawn("Alex", SchedulerHandout.Scheduler.Priority.High);
            _uut.Spawn("Nanna", SchedulerHandout.Scheduler.Priority.High);
            _uut.Spawn("Jeba", SchedulerHandout.Scheduler.Priority.High);
            Assert.That(_uut.GetNThreads(SchedulerHandout.Scheduler.Priority.High), Is.EqualTo(3));
        }

        [Test]
        public void GetNThreads_Spawn3Med_return3()
        {
            _uut.Spawn("Alex", SchedulerHandout.Scheduler.Priority.Med);
            _uut.Spawn("Nanna", SchedulerHandout.Scheduler.Priority.Med);
            _uut.Spawn("Jeba", SchedulerHandout.Scheduler.Priority.Med);
            Assert.That(_uut.GetNThreads(SchedulerHandout.Scheduler.Priority.Med), Is.EqualTo(3));
        }

        [Test]
        public void GetNThreads_Spawn3Low_return3()
        {
            _uut.Spawn("Alex", SchedulerHandout.Scheduler.Priority.Low);
            _uut.Spawn("Nanna", SchedulerHandout.Scheduler.Priority.Low);
            _uut.Spawn("Jeba", SchedulerHandout.Scheduler.Priority.Low);
            Assert.That(_uut.GetNThreads(SchedulerHandout.Scheduler.Priority.Low), Is.EqualTo(3));
        }

        [Test]
        public void GetNThreads_Spawn3Low2High_return2High()
        {
            _uut.Spawn("Alex", SchedulerHandout.Scheduler.Priority.Low);
            _uut.Spawn("Nanna", SchedulerHandout.Scheduler.Priority.Low);
            _uut.Spawn("Jeba", SchedulerHandout.Scheduler.Priority.Low);
            _uut.Spawn("Computer", SchedulerHandout.Scheduler.Priority.High);
            _uut.Spawn("Raspberry", SchedulerHandout.Scheduler.Priority.High);
            Assert.That(_uut.GetNThreads(SchedulerHandout.Scheduler.Priority.High), Is.EqualTo(2));
        }

        [Test]
        public void GetNThreads_spawn0_return0()
        {
            Assert.That(_uut.GetNThreads(SchedulerHandout.Scheduler.Priority.High), Is.EqualTo(0));
        }

        #endregion


    }
}
