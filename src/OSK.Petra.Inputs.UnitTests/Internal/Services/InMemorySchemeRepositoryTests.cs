using OSK.Petra.Inputs.Abstractions.Configuration;
using OSK.Petra.Inputs.Abstractions.Runtime;
using OSK.Petra.Inputs.Internal.Services;

namespace OSK.Petra.Inputs.UnitTests.Internal.Services;

public class InMemorySchemeRepositoryTests
{
    #region Variables

    private readonly InMemorySchemeRepository _repository;

    #endregion

    #region Constructors

    public InMemorySchemeRepositoryTests()
    {
        _repository = new InMemorySchemeRepository();
    }

    #endregion

    #region AllowCustomSchemes

    [Fact]
    public void AllowCustomSchemes_ReturnsFalse()
    {
        // Act
        Assert.False(_repository.AllowCustomSchemes);
    }

    #endregion

    #region SavePreferredSchemeAsync

    [Fact]
    public async Task SavePreferredSchemeAsync_NewUser_CreatesEntry()
    {
        // Arrange
        var scheme = new PreferredInputScheme() { UserId = 1, DefinitionName = "Default", SchemeName = "TestScheme", ConfigurationId = "config-1" };

        // Act
        var result = await _repository.SavePreferredSchemeAsync(scheme, TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Equal(scheme, result.Data);
    }

    [Fact]
    public async Task SavePreferredSchemeAsync_SameUserMultipleSchemes_StoresAll()
    {
        // Arrange
        var scheme1 = new PreferredInputScheme() { UserId = 1, DefinitionName = "Default", SchemeName = "Scheme1", ConfigurationId = "config-1" };
        var scheme2 = new PreferredInputScheme() { UserId = 1, DefinitionName = "Default2", SchemeName = "Scheme2", ConfigurationId = "config-1" };

        // Act
        await _repository.SavePreferredSchemeAsync(scheme1, TestContext.Current.CancellationToken);
        await _repository.SavePreferredSchemeAsync(scheme2, TestContext.Current.CancellationToken);

        // Assert
        var allSchemes = await _repository.GetPreferredSchemesAsync(TestContext.Current.CancellationToken);
        Assert.True(allSchemes.IsSuccessful);
        Assert.Equal(2, allSchemes.Data.Count());
    }

    [Fact]
    public async Task SavePreferredSchemeAsync_SameUserDifferentDefinition_StoredSeparately()
    {
        // Arrange
        var scheme1 = new PreferredInputScheme() { UserId = 1, DefinitionName = "Default", SchemeName = "Scheme1", ConfigurationId = "config-1" };
        var scheme2 = new PreferredInputScheme() { UserId = 1, DefinitionName = "Secondary", SchemeName = "Scheme2", ConfigurationId = "config-1" };

        // Act
        await _repository.SavePreferredSchemeAsync(scheme1, TestContext.Current.CancellationToken);
        await _repository.SavePreferredSchemeAsync(scheme2, TestContext.Current.CancellationToken);

        // Assert
        var allSchemes = await _repository.GetPreferredSchemesAsync(TestContext.Current.CancellationToken);
        Assert.True(allSchemes.IsSuccessful);
        Assert.Equal(2, allSchemes.Data.Count());
    }

    [Fact]
    public async Task SavePreferredSchemeAsync_DuplicateDefinitionName_ReplacesExisting()
    {
        // Arrange
        var scheme1 = new PreferredInputScheme() { UserId = 1, DefinitionName = "Default", SchemeName = "OldScheme", ConfigurationId = "config-1" };
        var scheme2 = new PreferredInputScheme() { UserId = 1, DefinitionName = "Default", SchemeName = "NewScheme", ConfigurationId = "config-1" };

        // Act
        await _repository.SavePreferredSchemeAsync(scheme1, TestContext.Current.CancellationToken);
        await _repository.SavePreferredSchemeAsync(scheme2, TestContext.Current.CancellationToken);

        // Assert
        var allSchemes = await _repository.GetPreferredSchemesAsync(TestContext.Current.CancellationToken);
        Assert.True(allSchemes.IsSuccessful);
        var userSchemes = allSchemes.Data.Where(s => s.UserId == 1 && s.DefinitionName == "Default").ToList();
        Assert.Single(userSchemes);
        Assert.Equal("NewScheme", userSchemes[0].SchemeName);
    }

    #endregion

    #region GetPreferredSchemesAsync

    [Fact]
    public async Task GetPreferredSchemesAsync_NoSchemes_ReturnsEmpty()
    {
        // Act
        var result = await _repository.GetPreferredSchemesAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Empty(result.Data);
    }

    [Fact]
    public async Task GetPreferredSchemesAsync_WithSchemes_ReturnsAll()
    {
        // Arrange
        var scheme = new PreferredInputScheme() { UserId = 1, DefinitionName = "Default", SchemeName = "TestScheme", ConfigurationId = "config-1" };
        await _repository.SavePreferredSchemeAsync(scheme, TestContext.Current.CancellationToken);

        // Act
        var result = await _repository.GetPreferredSchemesAsync(TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsSuccessful);
        Assert.Single(result.Data);
    }

    #endregion

    #region DeleteCustomSchemeAsync

    [Fact]
    public async Task DeleteCustomSchemeAsync_ThrowsNotImplemented()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() => _repository.DeleteCustomSchemeAsync("Default", "Test", TestContext.Current.CancellationToken));
    }

    #endregion

    #region GetCustomSchemesAsync

    [Fact]
    public async Task GetCustomSchemesAsync_ThrowsNotImplemented()
    {
        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() => _repository.GetCustomSchemesAsync(TestContext.Current.CancellationToken));
    }

    #endregion

    #region SaveCustomInputScheme

    [Fact]
    public async Task SaveCustomInputScheme_ThrowsNotImplemented()
    {
        // Arrange
        var scheme = new CustomInputScheme() { DefinitionName = "Default", Name = "Test", DeviceMaps = [] };

        // Act & Assert
        await Assert.ThrowsAsync<NotImplementedException>(() => _repository.SaveCustomInputScheme(scheme, TestContext.Current.CancellationToken));
    }

    #endregion
}
