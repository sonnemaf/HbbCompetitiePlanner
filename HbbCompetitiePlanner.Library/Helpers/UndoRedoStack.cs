using System;
using System.Collections.Generic;
using System.Linq;

namespace ReflectionIT.Universal.Helpers {

    public class UndoRedoStack {

        // Command object which holds a matching
        // do and undo operation
        private class Command {

            protected Command(string text) {
                this.Text = text;
            }

            public Command(Action redo, Action undo, string text) : this(text) {
                this.Redo = redo;
                this.Undo = undo;
            }

            public Action Redo { get; protected set; }
            public Action Undo { get; protected set; }

            public string Text { get; }

            public bool IsSavePoint { get; set; }

            public override string ToString() {
                return Text ?? base.ToString() + (IsSavePoint ? " (SP)" : string.Empty);
            }
        }

        private class BatchCommand : Command {

            public List<Command> CommandList { get; } = new List<Command>();
            public UndoRedStackBatch UndoRedStackBatch { get; }

            public BatchCommand(UndoRedStackBatch undoRedStackBatch, string text) : base(text) {

                this.Redo = delegate {
                    foreach (var item in this.CommandList) {
                        item.Redo();
                    }
                };
                this.Undo = delegate {
                    foreach (var item in ((IEnumerable<Command>)this.CommandList).Reverse()) {
                        item.Undo();
                    }
                };
                this.UndoRedStackBatch = undoRedStackBatch;
            }
        }

        // undo and redo stacks of Command objects
        private Stack<Command> UndoStack { get; } = new Stack<Command>();
        private Stack<Command> RedoStack { get; } = new Stack<Command>();
        private BatchCommand _batch;
        public RelayCommand<string> AddSavePointCommand { get; }
        public RelayCommand RollbackToSavePointCommand { get; }
        public IRelayCommand UndoCommand { get; }
        public IRelayCommand RedoCommand { get; }

        /// <summary>
        /// Constructor
        /// </summary>
        public UndoRedoStack()
            : base() {

            this.UndoCommand = new RelayCommand(() => {
                if (UndoStack.Count > 0) {
                    var cmd = UndoStack.Pop();
                    RedoStack.Push(cmd);
                    cmd.Undo();
                    CanExecuteChanged();
                }
            },
            () => UndoStack.Count > 0);

            this.RedoCommand = new RelayCommand(() => {
                if (RedoStack.Count > 0) {
                    var cmd = RedoStack.Pop();
                    UndoStack.Push(cmd);
                    cmd.Redo();
                    CanExecuteChanged();
                }
            },
            () => RedoStack.Count > 0);

            this.AddSavePointCommand = new RelayCommand<string>(AddSavePoint, text => this.UndoStack.Count > 0 && !this.UndoStack.First().IsSavePoint);
            this.RollbackToSavePointCommand = new RelayCommand(RollbackSavePoint, () => {
                return this.UndoStack.Any(cmd => cmd.IsSavePoint)
                    ? this.UndoStack.Multiple(c => c.IsSavePoint) ? true : !this.UndoStack.First().IsSavePoint
                    : false;
            });
        }

        private void CanExecuteChanged() {
            if (_batch is null) {
                this.UndoCommand.RaiseCanExecuteChanged();
                this.RedoCommand.RaiseCanExecuteChanged();
                this.AddSavePointCommand.RaiseCanExecuteChanged();
                this.RollbackToSavePointCommand.RaiseCanExecuteChanged();
            }
        }

#pragma warning disable RIT0007 // Optional parameters in public methods of public types.
        public void AddAndPerformOperation(Action operation, Action undoOperation, string text = "") {
#pragma warning restore RIT0007 // Optional parameters in public methods of public types.
            // Setup the cmd object to
            // contain our operations
            var cmd = new Command(operation, undoOperation, text);

            if (_batch is null) {
                AddAndPerformOperation(cmd);
            } else {
                // do the operation the first time
                operation();

                _batch.CommandList.Add(cmd);
            }
        }

        private void AddAndPerformOperation(Command cmd) {
            // push the command on the undo stack
            UndoStack.Push(cmd);

            // clear our redo "history"
            ClearRedoStack();

            // do the operation the first time
            cmd.Redo();

            CanExecuteChanged();
        }

        public void ClearRedoStack() {
            RedoStack.Clear();
        }

        public void Clear() {
            UndoStack.Clear();
            RedoStack.Clear();
            CanExecuteChanged();
        }

#pragma warning disable RIT0007 // Optional parameters in public methods of public types.
        public UndoRedStackBatch StartBatch(string text = "") {
#pragma warning restore RIT0007 // Optional parameters in public methods of public types.
            if (_batch is null) {
                _batch = new BatchCommand(new UndoRedStackBatch(this), text);
            } else {
                _batch.UndoRedStackBatch.Level++;
            }

            return _batch.UndoRedStackBatch;
        }

        internal void CommitBatch() {
            if (_batch is null) {
                throw new InvalidOperationException("Batch is not started");
            }
            // Push the command on the undo stack
            // Can't use AddAndPerformOperation() because the Redo operation may
            // not be exucuted (again).
            UndoStack.Push(_batch);

            ClearRedoStack();

            _batch = null;
            CanExecuteChanged();

            //this.AddAndPerformOperation(_batch);
        }

        /// <summary>
        /// Removes the Last Undo action which may be used in the AddAndPerform-Operation to cancel the Add
        /// </summary>
        public void RemoveLastUndo() {
            UndoStack.Pop();
            CanExecuteChanged();
        }

        private void AddSavePoint(string name) {
            if (_batch is null) {
                var cmd = this.UndoStack.FirstOrDefault();
                if (!(cmd is null) && !cmd.IsSavePoint) {
                    cmd.IsSavePoint = true;
                }
            } else {
                throw new InvalidOperationException("Batch is not committed, SavePoints are not allowed");
            }
        }

        private void RollbackSavePoint() {
            if (_batch is null) {
                var sp = this.UndoStack.FirstOrDefault(c => c.IsSavePoint);
                if (!(sp is null) && this.UndoStack.Count > 1) {
                    if (this.UndoStack.First() == sp) {
                        this.UndoCommand.Execute(null);
                    }
                    var cmds = this.UndoStack.TakeWhile(c => !c.IsSavePoint).ToList();
                    foreach (var item in cmds) {
                        this.UndoCommand.Execute(null);
                    }
                }
                this.CanExecuteChanged();
            } else {
                throw new InvalidOperationException("Batch is not committed, SavePoints are not allowed");
            }
        }
    }

    public class UndoRedStackBatch : IDisposable {

        internal int Level { get; set; }
        private readonly UndoRedoStack _stack;

        internal UndoRedStackBatch(UndoRedoStack stack) {
            this._stack = stack;
            this.Level = 1;
        }

        public void Dispose() {
            if (--Level == 0) {
                // Free managed resources
                this._stack.CommitBatch();
            }
        }
    }
}
