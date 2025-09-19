using Kamgam.UIToolkitVisualScripting;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UIElements;
using VV.Utility;

namespace VV.UI
{
    public class UIBaseElement : MonoBehaviour
    {
        #region Events

        public UnityEvent<VisualElement> onShow;
        public UnityEvent<VisualElement> onHide;

        #endregion
        
        #region Variables

        protected UIDocument doc;
        
        protected bool isVisible = true;
        public bool IsVisible() => isVisible;

        [SerializeField] private UIDocument document;
        [SerializeField] private string rootStringID;
        protected VisualElement Root => string.IsNullOrEmpty(rootStringID) ? 
            doc.rootVisualElement : doc.rootVisualElement.Q(rootStringID);

        private VisualElement selectedVisualElement;

        protected T Get<T>(string label) where T : VisualElement => Root.Q<T>(label);
        protected VisualElement Get(string label) => Root.Q<VisualElement>(label);
        protected Label GetLabel(string label) => Root.Q<Label>(label);
        protected Button GetButton(string label) => Root.Q<Button>(label);
        protected GroupBox GetGroup(string label) => Root.Q<GroupBox>(label);

        #endregion

        #region Triggers

        protected void Awake()
        {
            doc = document ?? GetComponent<UIDocument>();

            if(doc)
                selectedVisualElement = Root;
        }

        public void SetSelectedVisualElement(string veName) => SetSelectedVisualElement(Get(veName));
        public void SetSelectedVisualElement(VisualElement ve) => selectedVisualElement = ve;

        public virtual void Show() => Show(Root);
        public virtual void ShowSelected() => Show(selectedVisualElement);
        public void Show(string veName) => Show(Get(veName));
        public void Show(VisualElement ve)
        {
            if (ve == null) return;

            ve.style.display = DisplayStyle.Flex;
            
            onShow?.Invoke(ve);
        }

        public virtual void Hide() => Hide(Root, true);
        public virtual void Hide(bool shouldNotify) => Hide(Root, shouldNotify);
        public virtual void HideSelected(bool shouldNotify = true) => Hide(selectedVisualElement, shouldNotify);
        public virtual void HideSelectedAfterDelay(float delay) => this.Invoke(() => Hide(selectedVisualElement), delay);
        public void Hide(string veName) => Hide(Get(veName));

        public void Hide(string veName, bool shouldNotify) => Hide(Get(veName), shouldNotify);
        public void Hide(VisualElement ve, bool shouldNotify = true)
        {
            if (ve == null) return;

            ve.style.display = DisplayStyle.None;
            
            if(shouldNotify) onHide?.Invoke(ve);
        }
        
        public void FadeIn() { if(Root != null) FadeIn(Root); }
        public void FadeInSelected() => FadeIn(selectedVisualElement);
        public void FadeIn(string veName) => FadeIn(Get(veName));
        public void FadeIn(VisualElement ve)
        {
            Show();
            ve.RemoveFromClassList("Hidden");
            ve.AddToClassList("Visible");
        }
    
        public void FadeOut() { if(Root != null) FadeOut(Root);}
        public void FadeOutSelected() => FadeOut(selectedVisualElement);
        public void FadeOutSelectedAfterDelay(float delay) => this.Invoke(() => FadeOut(selectedVisualElement), delay);
        public void FadeOut(string veName) => FadeOut(Get(veName));
        public void FadeOut(VisualElement ve)
        {
            if (ve == null) return;
            
            ve.RemoveFromClassList("Visible");
            ve.AddToClassList("Hidden");
            ve.RegisterCallbackOnce<TransitionEndEvent>(_ =>
            {
                if(ve.HasClass("Hidden"))
                    Hide();
            });
        }
        
        
        public void SetImageBackground(string veName, Sprite img) => SetImageBackground(Get(veName), img);
        public void SetImageBackground(VisualElement ve, Sprite img) => ve?.SetBackgroundImage(img);
        
        
        public void AddClassToSelected(string className) => selectedVisualElement.AddToClassList(className);
        
        public void RemoveClassFromSelected(string className) => selectedVisualElement.RemoveFromClassList(className);

        #endregion

        public void SetDataSource(object dataSource) => Root.dataSource = dataSource;
    }
}