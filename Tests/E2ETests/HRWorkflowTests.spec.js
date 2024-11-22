test('TestHRAccessesDashboardAndGeneratesReports_EndToEnd_Success', async ({ page }) => {
    // Log in as HR
    await page.goto('http://localhost:5000/Account/Login');
    await page.fill('#Email', 'hr@example.com');
    await page.fill('#Password', 'password123');
    await page.click('#LoginButton');

    // Access the HR dashboard
    await page.goto('http://localhost:5000/HR/HRDashboard');
    await expect(page.locator('h1')).toHaveText('HR Dashboard');

    // Generate a lecturer activity report
    await page.click('#GenerateLecturerActivityReportButton');
    const [download] = await Promise.all([
        page.waitForEvent('download'),
        page.click('#DownloadLecturerReportLink')
    ]);

    const content = await download.text();
    expect(content).toContain('Lecturer ID');
    expect(content).toContain('Total Work Hours');
});
