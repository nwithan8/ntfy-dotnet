## help - Display help about make targets for this Makefile
help:
	@cat Makefile | grep '^## ' --color=never | cut -c4- | sed -e "`printf 's/ - /\t- /;'`" | column -s "`printf '\t'`" -t

## build - Build the project in Debug mode
build:
	dotnet msbuild -property:Configuration="Debug" -target:Rebuild -restore

## build-test-fw - Build the project for unit testing in Debug mode for a specific framework
# @parameters:
# fw= - The framework to build for.
build-test-fw:
	dotnet msbuild ntfy.Tests/ntfy.Tests.csproj -property:Configuration="Debug" -target:Rebuild -restore -property:TargetFramework=${fw}

## build-prod - Build the project in Release mode
build-prod:
	dotnet msbuild -property:Configuration="Release" -target:Rebuild -restore

## clean - Clean the project
clean:
	dotnet clean
	rm -rf *.nupkg

## coverage - Generate coverage reports for the project
coverage:
	./generate_test_reports.sh

## docs - Generates library documentation
docs:
	docfx docs/docfx.json
	git add docs/*

## format - Formats the project
format:
	dotnet dotnet-format --no-restore

## install-tools - Install required dotnet tools
install-tools:
	dotnet new tool-manifest || exit 0
	dotnet tool install --local security-scan --version 5.6.3 || exit 0
	dotnet tool install --local dotnet-format || exit 0
	dotnet tool install --local docfx || exit 0

## install - Install requirements
install: | install-tools
	git submodule init
	git submodule update

## lint - Lints the project
lint:
	dotnet dotnet-format --no-restore --check

## prep-release - Build and package a project for distribution
# @parameters:
# version= - The version to build of the project.
prep-release:
	dotnet pack -c Release -o output ntfy/*.csproj -p:Version=${version} -p:ContinuousIntegrationBuild=true

## restore - Restore the project
restore:
	dotnet restore

## scan - Scan the project for security issues (must run install-scanner first)
# Makefile cannot access global dotnet tools, so you need to run the below command manually.
scan:
	dotnet tool run security-scan --verbose --no-banner --ignore-msbuild-errors ntfy.sln
    # "--ignore-msbuild-errors" needed since MSBuild does not like F#: https://github.com/security-code-scan/security-code-scan/issues/235

## setup-win - Install required .NET versions and tools (Windows only)
setup-win:
	scripts\setup.bat

## setup-unix - Install required .NET versions and tools (Unix only)
setup-unix:
	bash scripts/setup.sh

## test - Test the project
test:
	dotnet test

## test-fw - Run the unit tests for a specific framework
# @parameters:
# fw= - The framework to build for.
test-fw:
    # Note, running .NET Framework tests on a non-Windows machine may cause issues: https://xunit.net/docs/getting-started/netfx/cmdline
	dotnet test ntfy.Tests/ntfy.Tests.csproj -f ${fw}

## uninstall-scanner - Uninstall SecurityCodeScan from your system
uninstall-scanner:
	dotnet tool uninstall security-scan

.PHONY: help build build-test-fw build-prod clean docs format install-tools install lint prep-release publish-all publish restore scan setup-win setup-unix test test-fw uninstall-scanner