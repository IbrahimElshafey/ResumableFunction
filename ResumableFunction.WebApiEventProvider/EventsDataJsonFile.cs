using Newtonsoft.Json;
using ResumableFunction.Abstraction.WebApiEventProvider.InOuts;
using ResumableFunction.WebApiEventProvider.InOuts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ResumableFunction.WebApiEventProvider
{
    public class EventsDataJsonFile : IDisposable, IEventsData
    {
        private Dictionary<string, ApiCallEvent>? _activeCalls = new Dictionary<string, ApiCallEvent>();
        private readonly ApiCallsData? _data;
        private string _dataFile = $"{Path.GetTempPath()}{Extensions.GetEventProviderName()}-data.json";
        private PeriodicTimer timer;
        private DateTime _lastChangeDate = DateTime.Now;
        private const int periodicSaveTime = 5;
        public EventsDataJsonFile()
        {
            if (File.Exists(_dataFile))
            {
                _data = JsonConvert.DeserializeObject<ApiCallsData>(File.ReadAllText(_dataFile));
                if(_data.IsStarted) 
                    SetStarted();
                else
                    SetStopped();
            }
            else
                _data = new ApiCallsData();
        }

        public Dictionary<string, ApiCallEvent> ActiveCalls
        {
            get
            {
                if (_activeCalls == null)
                    _activeCalls = new Dictionary<string, ApiCallEvent>();
                return _activeCalls;
            }
        }

        public Task<bool> AddActionPath(string actionPath)
        {
            var isAdded = _data.ActionPaths.Add(actionPath);
            if (isAdded)
                _data.Changed();
            return Task.FromResult(isAdded);
        }

        public Task<bool> DeleteActionPath(string actionPath)
        {
            var isDeleted = _data.ActionPaths.Remove(actionPath);
            if (isDeleted)
                _data.Changed();
            return Task.FromResult(isDeleted);
        }


        public Task<bool> IsStarted()
        {
            return Task.FromResult(_data.IsStarted);
        }

        public async Task SetStarted()
        {
            if (!_data.IsStarted)
            {
                _data.IsStarted = true;
                _data.Changed();
            }
            timer = new PeriodicTimer(TimeSpan.FromSeconds(periodicSaveTime));
            while (await timer.WaitForNextTickAsync() && _data.IsStarted)
            {
                if (_data.LastChangeDate > _lastChangeDate)
                    await SaveDataToJsonFile();
            }
            //return Task.CompletedTask;
        }

        private async Task SaveDataToJsonFile()
        {
            await File.WriteAllTextAsync(_dataFile, JsonConvert.SerializeObject(_data));
            _lastChangeDate = DateTime.Now;
        }

        public async Task SetStopped()
        {
            if (_data.IsStarted)
            {
                _data.IsStarted = false;
                _data.Changed();
            }
            await SaveDataToJsonFile();
            timer?.Dispose();
        }

        public Task<bool> IsSubscribedToAction(string eventIdentifier)
        {
            return Task.FromResult(_data.ActionPaths.Contains(eventIdentifier));
        }

        public void Dispose()
        {
            timer?.Dispose();
        }


    }
}
