﻿using Livet;
using Livet.Commands;
using NodeGraph.CommandParameters;
using NodeGraph.PreviewTest.ViewModels;
using NodeGraph.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace NodeGraph.PreviewTest.ViewModels
{
    public enum GroupIntersectType
    {
        CursorPoint,
        BoundingBox,
    }

    public class MainWindowViewModel : ViewModel
    {
        public ViewModelCommand AddNodeCommand => _AddNodeCommand.Get(AddNode);
        ViewModelCommandHandler _AddNodeCommand = new ViewModelCommandHandler();

        public ViewModelCommand AddGroupNodeCommand => _AddGroupNodeCommand.Get(AddGroupNode);
        ViewModelCommandHandler _AddGroupNodeCommand = new ViewModelCommandHandler();

        public ViewModelCommand RemoveNodesCommand => _RemoveNodesCommand.Get(RemoveNodes);
        ViewModelCommandHandler _RemoveNodesCommand = new ViewModelCommandHandler();

        public ListenerCommand<PreviewConnectCommandParameter> PreviewConnectCommand => _PreviewConnectCommand.Get(PreviewConnect);
        ViewModelCommandHandler<PreviewConnectCommandParameter> _PreviewConnectCommand = new ViewModelCommandHandler<PreviewConnectCommandParameter>();

        public ListenerCommand<ConnectedCommandParameter> ConnectedCommand => _ConnectedCommand.Get(Connected);
        ViewModelCommandHandler<ConnectedCommandParameter> _ConnectedCommand = new ViewModelCommandHandler<ConnectedCommandParameter>();

        public ListenerCommand<DisconnectedCommandParameter> DisconnectedCommand => _DisconnectedCommand.Get(Disconnected);
        ViewModelCommandHandler<DisconnectedCommandParameter> _DisconnectedCommand = new ViewModelCommandHandler<DisconnectedCommandParameter>();

        public ListenerCommand<NodesMovedCommandParameter> NodesMovedCommand => _NodesMovedCommand.Get(NodesMoved);
        ViewModelCommandHandler<NodesMovedCommandParameter> _NodesMovedCommand = new ViewModelCommandHandler<NodesMovedCommandParameter>();

        public ListenerCommand<IList> SelectionChangedCommand => _SelectionChangedCommand.Get(SelectionChanged);
        ViewModelCommandHandler<IList> _SelectionChangedCommand = new ViewModelCommandHandler<IList>();

        public ViewModelCommand AddTestNodeLinkCommand => _AddTestNodeLinkCommand.Get(AddTestNodeLink);
        ViewModelCommandHandler _AddTestNodeLinkCommand = new ViewModelCommandHandler();

        public ViewModelCommand MoveTestNodesCommand => _MoveTestNodesCommand.Get(MoveTestNodes);
        ViewModelCommandHandler _MoveTestNodesCommand = new ViewModelCommandHandler();

        public ViewModelCommand ClearTestNodesCommand => _ClearTestNodesCommand.Get(ClearTestNodes);
        ViewModelCommandHandler _ClearTestNodesCommand = new ViewModelCommandHandler();

        public ViewModelCommand MoveGroupNodeCommand => _MoveGroupNodeCommand.Get(MoveGroupNode);
        ViewModelCommandHandler _MoveGroupNodeCommand = new ViewModelCommandHandler();

        public ViewModelCommand ChangeGroupInnerSizeCommand => _ChangeGroupInnerSizeCommand.Get(ChangeGroupInnerSize);
        ViewModelCommandHandler _ChangeGroupInnerSizeCommand = new ViewModelCommandHandler();

        public ViewModelCommand ChangeGroupInnerPositionCommand => _ChangeGroupInnerPositionCommand.Get(ChangeGroupInnerPosition);
        ViewModelCommandHandler _ChangeGroupInnerPositionCommand = new ViewModelCommandHandler();

        public IEnumerable<NodeBaseViewModel> NodeViewModels => _NodeViewModels;
        ObservableCollection<NodeBaseViewModel> _NodeViewModels = new ObservableCollection<NodeBaseViewModel>();

        public IEnumerable<NodeLinkViewModel> NodeLinkViewModels => _NodeLinkViewModels;
        ObservableCollection<NodeLinkViewModel> _NodeLinkViewModels = new ObservableCollection<NodeLinkViewModel>();

        public IEnumerable<GroupNodeViewModel> GroupNodeViewModels => _GroupNodeViewModels;
        ObservableCollection<GroupNodeViewModel> _GroupNodeViewModels = new ObservableCollection<GroupNodeViewModel>();

        public GroupIntersectType[] GroupIntersectTypes => Enum.GetValues(typeof(GroupIntersectType)).OfType<GroupIntersectType>().ToArray();

        public GroupIntersectType SelectedGroupIntersectType
        {
            get => _SelectedGroupIntersectType;
            set => RaisePropertyChangedIfSet(ref _SelectedGroupIntersectType, value);
        }
        GroupIntersectType _SelectedGroupIntersectType;

        public bool IsLockedAllNodeLinks
        {
            get => _IsLockedAllNodeLinks;
            set => UpdateIsLockedAllNodeLinksProperty(value);
        }
        bool _IsLockedAllNodeLinks = false;

        public bool IsEnableAllNodeConnectors
        {
            get => _IsEnableAllNodeConnectors;
            set => UpdateIsEnableAllNodeConnectorsProperty(value);
        }
        bool _IsEnableAllNodeConnectors = true;


        public MainWindowViewModel()
        {
            _GroupNodeViewModels.Add(new GroupNodeViewModel() { Name = "Group1" });
            _NodeViewModels.Add(new NodeViewModel1() { Name = "Node1", Body = "Content1", Position = new Point(0, 100) });
            _NodeViewModels.Add(new NodeViewModel1() { Name = "Node2", Body = "Content2", Position = new Point(100, 200) });
            _NodeViewModels.Add(new NodeViewModel1() { Name = "Node3", Body = "Content3", Position = new Point(200, 300) });
        }

        void AddNode()
        {
            _NodeViewModels.Add(new NodeViewModel2() { Name = "NewNode", Body = "NewContent" });
        }

        void AddGroupNode()
        {
            _GroupNodeViewModels.Add(new GroupNodeViewModel() { Name = "NewGroupNode" });
        }

        void RemoveNodes()
        {
            var removeNodes = _NodeViewModels.Where(arg => arg.IsSelected).ToArray();
            foreach (var removeNode in removeNodes)
            {
                _NodeViewModels.Remove(removeNode);
            }
        }

        void ClearTestNodes()
        {
            _NodeViewModels.Clear();
        }

        void MoveGroupNode()
        {
            _GroupNodeViewModels[0].InterlockPosition = new Point(0, 0);
        }

        void ChangeGroupInnerSize()
        {
            _GroupNodeViewModels[0].InnerWidth = 300;
            _GroupNodeViewModels[0].InnerHeight = 300;
        }

        void ChangeGroupInnerPosition()
        {
            _GroupNodeViewModels[0].InnerPosition = new Point(0, 0);
        }

        void UpdateIsLockedAllNodeLinksProperty(bool value)
        {
            _IsLockedAllNodeLinks = !_IsLockedAllNodeLinks;

            foreach (var nodeLink in _NodeLinkViewModels)
            {
                nodeLink.IsLocked = _IsLockedAllNodeLinks;
            }

            RaisePropertyChanged(nameof(IsLockedAllNodeLinks));
        }

        void UpdateIsEnableAllNodeConnectorsProperty(bool value)
        {
            _IsEnableAllNodeConnectors = !_IsEnableAllNodeConnectors;

            foreach (var node in _NodeViewModels)
            {
                foreach (var input in node.Inputs)
                {
                    input.IsEnable = _IsEnableAllNodeConnectors;
                }
                foreach (var output in node.Outputs)
                {
                    output.IsEnable = _IsEnableAllNodeConnectors;
                }
            }

            RaisePropertyChanged(nameof(IsEnableAllNodeConnectors));
        }

        void PreviewConnect(PreviewConnectCommandParameter param)
        {
            var inputNode = NodeViewModels.First(arg => arg.Guid == param.ConnectToEndNodeGuid);
            var inputConnector = inputNode.FindConnector(param.ConnectToEndConnectorGuid);
            param.CanConnect = inputConnector.Label == "Limited Input" == false;
        }

        void Connected(ConnectedCommandParameter param)
        {
            var nodeLink = new NodeLinkViewModel()
            {
                InputGuid = param.InputConnectorGuid,
                InputNodeGuid = param.InputNodeGuid,
                OutputGuid = param.OutputConnectorGuid,
                OutputNodeGuid = param.OutputNodeGuid,
            };
            _NodeLinkViewModels.Add(nodeLink);
        }

        void Disconnected(DisconnectedCommandParameter param)
        {
            var nodeLink = _NodeLinkViewModels.First(arg => arg.Guid == param.NodeLinkGuid);
            _NodeLinkViewModels.Remove(nodeLink);
        }

        void NodesMoved(NodesMovedCommandParameter param)
        {

        }

        void SelectionChanged(IList list)
        {

        }

        void AddTestNodeLink()
        {
            var nodeLink = new NodeLinkViewModel();
            nodeLink.OutputGuid = (_NodeViewModels[0] as NodeViewModel1).Outputs.ElementAt(0).Guid;
            nodeLink.InputGuid = (_NodeViewModels[1] as NodeViewModel1).Inputs.ElementAt(0).Guid;
            _NodeLinkViewModels.Add(nodeLink);
        }

        void MoveTestNodes()
        {
            _NodeViewModels[0].Position = new Point(0, 0);
        }
    }
}
