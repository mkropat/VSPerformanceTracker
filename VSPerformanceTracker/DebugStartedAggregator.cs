using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using VSPerformanceTracker.EventResults;
using VSPerformanceTracker.IISInterface;
using VSPerformanceTracker.OSInterface;
using VSPerformanceTracker.VSInterface;

namespace VSPerformanceTracker
{
    public sealed class DebugStartAggregator : IDisposable
    {
        class DebuggerAggregatorEvent
        {
            public DateTime Start { get; set; }
            public DateTime? End { get; set; }
            public string Path { get; set; }
        }

        private readonly IBrowseToUrlQueryer _urlQueryer;
        private readonly ITimeService _timeService;
        private DebuggerAggregatorEvent _currentEvent;
        private readonly Subject<GenericEventResult> _events = new Subject<GenericEventResult>();
        private readonly IList<DebuggerAggregatorEvent> _bufferedEvents = new List<DebuggerAggregatorEvent>(); // FIXME: should be a circular buffer

        public DebugStartAggregator(IBrowseToUrlQueryer urlQueryer, ITimeService timeService)
        {
            _urlQueryer = urlQueryer;
            _timeService = timeService;
        }

        public IObservable<IEventResult> DebugStarted
        {
            get { return _events.AsObservable(); }
        }

        public void Aggregate(IObservable<DebuggerEvent> debuggerEvents, IObservable<IISLogEvent> logEvents)
        {
            debuggerEvents.Subscribe(OnDebuggerEvent);
            logEvents.Subscribe(OnLogEvent);
        }

        private void OnDebuggerEvent(DebuggerEvent evt)
        {
            switch (evt.Action)
            {
                case DebuggerAction.Start:
                    PushCurrentEvent();

                    _currentEvent = new DebuggerAggregatorEvent
                    {
                        Start = _timeService.GetCurrent(),
                        Path = _urlQueryer.GetCurrent(),
                    };

                    break;

                case DebuggerAction.Stop:
                    PushCurrentEvent();
                    break;
            }
        }

        private void OnLogEvent(IISLogEvent logEvent)
        {
            var startEvent = PopBufferedStartEvent(logEvent.Path, logEvent.Time);
            if (startEvent == null)
                return;

            _events.OnNext(new GenericEventResult
            {
                Start = startEvent.Start,
                Duration = logEvent.Time - startEvent.Start,
            });
        }

        private DebuggerAggregatorEvent PopBufferedStartEvent(string path, DateTime time)
        {
            if (_currentEvent != null && path == _currentEvent.Path && _currentEvent.Start < time)
            {
                var result = _currentEvent;
                _currentEvent = null;
                return result;
            }

            var startEvent = _bufferedEvents.Reverse().FirstOrDefault(se => path == se.Path && se.Start < time && time < se.End);
            if (startEvent == null)
                return null;

            _bufferedEvents.Remove(startEvent);
            return startEvent;
        }

        private void PushCurrentEvent()
        {
            if (_currentEvent == null)
                return;

            _currentEvent.End = _timeService.GetCurrent();
            _bufferedEvents.Add(_currentEvent);
            _currentEvent = null;
        }

        public void Dispose()
        {
            _events.Dispose();
        }
    }
}
