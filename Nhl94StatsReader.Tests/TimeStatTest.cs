// <copyright file="TimeStatTest.cs">Copyright ©  2015</copyright>
using System;
using Microsoft.Pex.Framework;
using Microsoft.Pex.Framework.Validation;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Nhl94StatsReader;

namespace Nhl94StatsReader.Tests
{
    /// <summary>This class contains parameterized unit tests for TimeStat</summary>
    [PexClass(typeof(TimeStat))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(InvalidOperationException))]
    [PexAllowedExceptionFromTypeUnderTest(typeof(ArgumentException), AcceptExceptionSubtypes = true)]
    [TestClass]
    public partial class TimeStatTest
    {
        /// <summary>Test stub for ReadStat()</summary>
        [PexMethod]
        public void ReadStatTest([PexAssumeUnderTest]TimeStat target)
        {
            target.ReadStat();
            // TODO: add assertions to method TimeStatTest.ReadStatTest(TimeStat)
        }

        /// <summary>Test stub for WriteStat()</summary>
        [PexMethod]
        public void WriteStatTest([PexAssumeUnderTest]TimeStat target)
        {
            target.WriteStat();
            // TODO: add assertions to method TimeStatTest.WriteStatTest(TimeStat)
        }
    }
}
