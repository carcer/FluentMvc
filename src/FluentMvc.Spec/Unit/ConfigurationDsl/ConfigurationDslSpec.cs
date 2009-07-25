namespace FluentMvc.Spec.Unit.ConfigurationDsl
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Web.Mvc;
    using Configuration;
    using Constraints;
    using FluentMvc;
    using FluentMvc.ActionResultFactories;
    using NUnit.Framework;
    using Rhino.Mocks;
    using Utils;

    [TestFixture]
    public class When_no_settings_have_been_set : DslSpecBase
    {
        public override void Because()
        {
            Configuration = FluentMvcConfiguration.Create();
        }

        [Test]
        public void Should_have_default_conventions()
        {
            Configuration.Convention.ShouldBeOfType(typeof(FluentMvcConvention));
        }
    }

    [TestFixture]
    public class when_registering_an_instance_of_a_filter_with_no_contraints : DslSpecBase
    {
        private ActionFilterRegistry actionFilterRegistry;
        private IActionFilter filterInstance;

        public override void Because()
        {
            actionFilterRegistry = new ActionFilterRegistry(CreateStub<IFluentMvcObjectFactory>());
            filterInstance = CreateStub<IActionFilter>();
            FluentMvcConfiguration.Create(CreateStub<IFluentMvcResolver>(), actionFilterRegistry, CreateStub<IActionResultRegistry>())
                .WithFilter(filterInstance).BuildControllerFactory();
        }

        [Test]
        public void should_be_registered_using_the_supplied_type()
        {
            actionFilterRegistry.Registrations.First().Create<IActionFilter>(CreateStub<IFluentMvcObjectFactory>()).ShouldEqual(filterInstance);
        }
    }


    [TestFixture]
    public class When_creating_a_factory_with_an_inner_controllerfactory : DslSpecBase
    {
        private IControllerFactory Factory;
        private DefaultControllerFactory ExpectedControllerFactory;

        public override void Given()
        {
            ExpectedControllerFactory = new DefaultControllerFactory();
            Configuration = FluentMvcConfiguration.Create()
                .UsingControllerFactory(ExpectedControllerFactory);
        }

        public override void Because()
        {
            Factory = Configuration
                .BuildControllerFactory();
        }

        [Test]
        public void Should_set_inner_conventions_controller_factory()
        {
            Configuration.Convention.ControllerFactory.ShouldBeTheSameAs(ExpectedControllerFactory);
        }

        [Test]
        public void Should_create_the_factory()
        {
            Factory.ShouldNotBeNull();
        }
    }

    [TestFixture]
    public class When_adding_two_action_result_factories : DslSpecBase
    {
        private IActionResultFactory Child1;
        private IActionResultFactory Child2;

        public override void Given()
        {
            Configuration = FluentMvcConfiguration.Create();
            Child1 = CreateStub<IActionResultFactory>();
            Child2 = CreateStub<IActionResultFactory>();
        }

        public override void Because()
        {
            Configuration
                .WithResultFactory(Child1)
                .WithResultFactory(Child2);
        }

        [Test]
        public void Should_set_inner_conventions_factories()
        {
            Configuration.Convention.Factories.ToArray()[0].ShouldBeTheSameAs(Child1);
            Configuration.Convention.Factories.ToArray()[1].ShouldBeTheSameAs(Child2);
        }
    }

    [TestFixture]
    public class When_adding_one_action_result_factory_generically : DslSpecBase
    {

        public override void Given()
        {
            Configuration = FluentMvcConfiguration.Create();
        }

        public override void Because()
        {
            Configuration
                .WithResultFactory<TestActionResultFactory>();
        }

        [Test]
        public void Should_set_inner_conventions_factories()
        {
            Configuration.Convention.Factories.ToArray()[0].ShouldBe(typeof (TestActionResultFactory));
        }
    }

    [TestFixture]
    public class When_adding_one_action_result_factories_with_a_constraint : DslSpecBase
    {
        private IActionResultFactory resultFactory;

        public override void Given()
        {
            resultFactory = CreateStub<IActionResultFactory>();
            Configuration = FluentMvcConfiguration.Create();
        }

        public override void Because()
        {
            Configuration
                .WithResultFactory(resultFactory, Apply.When<ExpectsJson>());
        }

        [Test]
        public void should_override_factory_constraint()
        {
            resultFactory.AssertWasCalled(x => x.SetConstraint(Arg<IEnumerable<IConstraint>>.Is.Anything));
        }
    }

    [TestFixture]
    public class When_overriding_the_convention : DslSpecBase
    {
        private FluentMvcConvention ExpectedConvention;

        public override void Given()
        {
            Configuration = FluentMvcConfiguration.Create();
            ExpectedConvention = CreateStub<FluentMvcConvention>();
        }

        public override void Because()
        {
            Configuration.WithConvention(ExpectedConvention);
        }

        [Test]
        public void Should_set_the_inner_convention()
        {
            Configuration.Convention.ShouldBeTheSameAs(ExpectedConvention);
        }
    }

    [TestFixture]
    public class When_editing_convention : DslSpecBase
    {
        private bool WasCalled;

        public override void Given()
        {
            Configuration = FluentMvcConfiguration.Create();
        }

        public override void Because()
        {
            Configuration.WithConvention(convention =>
                                             {
                                                 WasCalled = true;
                                             });
        }

        [Test]
        public void Should_invoke_action()
        {
            WasCalled.ShouldBeTrue();
        }
    }

    [TestFixture]
    public class When_setting_the_default_factory : DslSpecBase
    {
        private IActionResultFactory ExpectedDefaultFactory;

        public override void Given()
        {
            ExpectedDefaultFactory = CreateStub<IActionResultFactory>();
            Configuration = FluentMvcConfiguration.Create();
        }

        public override void Because()
        {
            Configuration.WithResultFactory(ExpectedDefaultFactory, true);
        }

        [Test]
        public void Should_set_the_inner_convention()
        {
            Configuration.Convention.DefaultFactory.ShouldBeTheSameAs(ExpectedDefaultFactory);
        }
    }

    [TestFixture]
    public class When_setting_the_default_factory_generically : DslSpecBase
    {
        private IActionResultFactory ExpectedDefaultFactory;

        public override void Given()
        {
            ExpectedDefaultFactory = CreateStub<IActionResultFactory>();
            Configuration = FluentMvcConfiguration.Create();
        }

        public override void Because()
        {
           Configuration.WithResultFactory<TestActionResultFactory>(true);
        }

        [Test]
        public void Should_set_the_inner_convention()
        {
            Configuration.Convention.DefaultFactory.ShouldBe(typeof(TestActionResultFactory));
        }
    }

    [TestFixture]
    public class when_building_controller_factory : DslSpecBase
    {
        private IFluentMvcResolver fluentMvcResolver;
        private IActionFilterRegistry actionFilterRegistry;
        private IActionResultRegistry actionResultRegistry;

        public override void Given()
        {
            fluentMvcResolver = CreateStub<IFluentMvcResolver>();
            actionFilterRegistry = CreateStub<IActionFilterRegistry>();
            actionResultRegistry = CreateStub<IActionResultRegistry>();

            Configuration = FluentMvcConfiguration
                .Create(fluentMvcResolver, actionFilterRegistry, actionResultRegistry);
        }

        public override void Because()
        {
            Configuration.BuildControllerFactory();
        }

        [Test]
        public void should_set_resolvers_action_result_registry()
        {
            fluentMvcResolver
                .AssertWasCalled(arr => arr.SetActionResultRegistry(Arg<IActionResultRegistry>.Is.Anything));
        }

        [Test]
        public void should_set_resolvers_action_filter_registry()
        {
            fluentMvcResolver
                .AssertWasCalled(arr => arr.SetActionFilterRegistry(Arg<IActionFilterRegistry>.Is.Anything));
        }

        [Test]
        public void should_set_resolvers_action_result_pipeline()
        {
            fluentMvcResolver
                .AssertWasCalled(arr => arr.RegisterActionResultPipeline(Arg<IActionResultPipeline>.Is.Anything));
        }
    }

    [TestFixture]
    public class when_registering_a_custom_object_factory_with_a_result_factory : DslSpecBase
    {
        private IFluentMvcObjectFactory objectFactory;
        private IActionFilterRegistry filterRegistry;

        public override void Given()
        {
            objectFactory = CreateStub<IFluentMvcObjectFactory>();
            filterRegistry = CreateStub<IActionFilterRegistry>();
            Configuration = FluentMvcConfiguration.Create(CreateStub<IFluentMvcResolver>(), filterRegistry, CreateStub<IActionResultRegistry>())
                .ResolveWith(objectFactory)
                .WithResultFactory<JsonResultFactory>();
        }

        public override void Because()
        {
            Configuration.BuildControllerFactory();
        }

        [Test]
        public void should_set_action_filter_registries_object_factory()
        {
            filterRegistry.AssertWasCalled(f => f.SetObjectFactory(Arg<IFluentMvcObjectFactory>.Is.Anything));
        }

        [Test]
        public void should_use_object_factory()
        {
            objectFactory.AssertWasCalled(o => o.CreateFactory<JsonResultFactory>());
        }

    }

    


}