﻿namespace Saritasa.Tools.Messages.Common.Expressions
{
    public class ExpressionExecutorFactory
    {
        private ExpressionExecutorServices executorServices;

        public ExpressionExecutorFactory(ExpressionExecutorServices executorServices)
        {
            this.executorServices = executorServices;
        }

        public ExpressionExecutor Create()
        {
            var compiledExpressionProvider = executorServices.CompiledCache;
            var expressionCompilator = executorServices.ExpressionCompilator;
            var transformVisitorFactory = executorServices.TransformVisitorFactory;
            var expressionReduceVisitor = executorServices.ReduceVisitor;

            return new ExpressionExecutor(compiledExpressionProvider, expressionCompilator, transformVisitorFactory, expressionReduceVisitor);
        }
    }
}
