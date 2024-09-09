# Endjin.RecommendedPractices.GitHubActions

This repository contains [re-usable GitHub Action workflows](https://docs.github.com/en/actions/using-workflows/reusing-workflows) and [composite actions](https://docs.github.com/en/actions/creating-actions/creating-a-composite-action) for our standardised CI processes.

Our standardised build process is divided into the following phases:
- Compile
- Test
- Analyse
- Package
- Publish

***NOTE**: By default, the 'Publish' phase is only executed for tagged versions or when manually triggered with the 'Force Publish' option enabled.*

## Reusable Workflows

### Multi-Job Workflows
These run the logical phases of the build process using discrete jobs, which can be beneficial for large builds due to the parallelisation when running tests and building packages.

- `scripted-build-pipeline` - encapsulates our standard CI build process, using separate jobs for Compile, Test, Package & Publish phases
- `scripted-build-matrix-pipeline` - as above, except the Test phase includes [matrix](https://docs.github.com/en/actions/writing-workflows/choosing-what-your-workflow-does/running-variations-of-jobs-in-a-workflow) support

The diagram below shows the high-level process that the non-matrix multi-job workflow implements:

```mermaid
graph LR
    compile["Compile"]-->analyse["Code Analysis"]
    analyse-->test["Run Tests"]
    test-->pubtests["Publish Test Results"] 
    analyse-->package["Build Packages"]
    pubtests-->publish["Publish Packages"]
    package-->publish
```

The diagram below illustrates the different high-level process for the matrix-enabled version of the multi-job workflow:

```mermaid
graph LR
    compile["Compile"]-->analyse["Code Analysis"]
    analyse-->test1["Run Tests (matrix 1)"]
    analyse-->test2["Run Tests (matrix 2)"]
    test1-->pubtests["Publish Test Results"] 
    test2-->pubtests["Publish Test Results"] 
    analyse-->package["Build Packages"]
    pubtests-->publish["Publish Packages"]
    package-->publish
```


### Single-Job Workflow
This runs the logical phases of the build process as a single job, closely mimicking the local developer build.

***NOTE**: A consuming build will still require a second job in order to support passing arbitrary environment variables and secrets to this workflow. See the [example](.github/workflows/ci-single-job.yml) for more details.*

- `scripted-build-single-job-pipeline` - encapsulates our standard CI build process, using a single job

This diagram shows the default sequence of the build process implemented by the single-job workflow implement (NOTE: This can altered by overriding the `buildTasks` input parameter):

```mermaid
graph LR
    compile["Compile"]-->test["Run Tests"]
    test-->analyse["Code Analysis"]
    analyse-->package["Build Packages"]
    package-->publish["Publish Packages"]
    publish-->pubtests["Publish Test Results"] 
```


## Composite Actions

### Orchestration Composite Actions
This are used as alternatives to reusable workflows to encapsulate complete processes.

- `run-build-process` - provides an alternative to using the reusable workflows and encapsulates the complete build process. This allows the consuming CI build to be run a single job, with the trade-off of requiring somewhat more boilerplate.

The `run-build-process` action implements the same logical build process as the [Single-Job Workflow](#single-job-workflows) (NOTE: This can altered by overriding the `buildTasks` input parameter)

```mermaid
graph LR
    compile["Compile"]-->test["Run Tests"]
    test-->analyse["Code Analysis"]
    analyse-->package["Build Packages"]
    package-->publish["Publish Packages"]
    publish-->pubtests["Publish Test Results"] 
```


### Feature Composite Actions
These composite actions are used to encapsulated specific functionality so it can be easily re-used between workflows.

- `prepare-env-vars-and-secrets` - provides a workaround for not natively being able to pass arbitrary environment variables and secrets to a reusable workflow. Based on assembling the required values into 2 well-known variables that act as containers for the variables and secrets that need to be passed.
- `run-scripted-build` - encapsulates the steps for executing our [PowerShell-based build tooling](https://www.powershellgallery.com/packages/Endjin.RecommendedPractices.Build) - typically used via one of the above reusable workflows.
- `set-env-vars-and-secrets` - the consuming side of the workaround for passing arbitrary environment variables and secrets. Unwraps the bundled environment variables and secrets so they are available to the running workflow.

## Examples

The following workflows serve as examples of how to consume the different reusable workflows and composite actions found in this repo:

- [ci.yml](.github/workflows/ci.yml) - used for validating changes to the `scripted-build-pipeline` reusable workflow
- [ci-matrix.yml](.github/workflows/ci-matrix.yml) - used for validating changes to the `scripted-build-matrix-pipeline` reusable workflow
- [ci-single-job.yml](.github/workflows/ci-single-job.yml) - used for validating changes to the `scripted-build-single-job-pipeline` reusable workflow
- [ci-composite-action](.github/workflows/ci-composite-action.yml) - used for validating change to the `run-build-process` composite action


