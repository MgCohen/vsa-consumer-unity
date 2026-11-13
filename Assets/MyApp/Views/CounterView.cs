using Counter.Presenters;
using UnityEngine;
using UnityEngine.UI;
using VSA.Unity;

namespace MyApp.Views
{
    public sealed class CounterView : PresenterComponent<CounterPresenter>
    {
        [SerializeField]
        private Text label;

        [SerializeField]
        private Button add;

        public Text Label => label;

        public Button Add => add;

        protected override void Start()
        {
            base.Start();
            if (Presenter == null)
            {
                label.text = "No VSA app runs.";
                return;
            }
            add.onClick.AddListener(AddOne);
            _ = Presenter.LoadAsync();
        }

        protected override void Render()
        {
            string count = $"{Presenter.Value} of {Presenter.Max}";
            string error = Presenter.Increment.Error ?? Presenter.Load.Error;
            label.text = error == null ? count : $"{count}: {error}";
        }

        private void AddOne()
        {
            _ = Presenter.IncrementAsync(1);
        }
    }
}
