using System;
using Microsoft.Synchronization;

namespace VwSyncSever
{
    class SynProvider : KnowledgeSyncProvider, IChangeDataRetriever, INotifyingChangeApplierTarget, IDisposable
    {
        public override SyncIdFormatGroup IdFormats
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override void BeginSession(SyncProviderPosition position, SyncSessionContext syncSessionContext)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            
        }

        public override void EndSession(SyncSessionContext syncSessionContext)
        {
            throw new NotImplementedException();
        }

        public override ChangeBatch GetChangeBatch(uint batchSize, SyncKnowledge destinationKnowledge, out object changeDataRetriever)
        {
            throw new NotImplementedException();
        }

        public IChangeDataRetriever GetDataRetriever()
        {
            return null;
        }

        public override FullEnumerationChangeBatch GetFullEnumerationChangeBatch(uint batchSize, SyncId lowerEnumerationBound, SyncKnowledge knowledgeForDataRetrieval, out object changeDataRetriever)
        {
            throw new NotImplementedException();
        }

        public ulong GetNextTickCount()
        {
            return 0;
        }

        public override void GetSyncBatchParameters(out uint batchSize, out SyncKnowledge knowledge)
        {
            throw new NotImplementedException();
        }

        public object LoadChangeData(LoadChangeContext loadChangeContext)
        {
            return null;
        }

        public override void ProcessChangeBatch(ConflictResolutionPolicy resolutionPolicy, ChangeBatch sourceChanges, object changeDataRetriever, SyncCallbacks syncCallbacks, SyncSessionStatistics sessionStatistics)
        {
            throw new NotImplementedException();
        }

        public override void ProcessFullEnumerationChangeBatch(ConflictResolutionPolicy resolutionPolicy, FullEnumerationChangeBatch sourceChanges, object changeDataRetriever, SyncCallbacks syncCallbacks, SyncSessionStatistics sessionStatistics)
        {
            throw new NotImplementedException();
        }

        public void SaveChangeWithChangeUnits(ItemChange change, SaveChangeWithChangeUnitsContext context)
        {
            
        }

        public void SaveConflict(ItemChange conflictingChange, object conflictingChangeData, SyncKnowledge conflictingChangeKnowledge)
        {
            
        }

        public void SaveItemChange(SaveChangeAction saveChangeAction, ItemChange change, SaveChangeContext context)
        {
            
        }

        public void StoreKnowledgeForScope(SyncKnowledge knowledge, ForgottenKnowledge forgottenKnowledge)
        {
            
        }

        public bool TryGetDestinationVersion(ItemChange sourceChange, out ItemChange destinationVersion)
        {
            destinationVersion = null;
            return false;
        }
    }
}
