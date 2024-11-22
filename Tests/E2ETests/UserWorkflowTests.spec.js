import { test, expect } from '@playwright/test';

test('TestLecturerSubmitsClaimAndDownloadsReport_EndToEnd_Success', async ({ page }) => {
    // Log in as lecturer
    await page.goto('http://localhost:5000/Account/Login');
    await page.fill('#Email', 'lecturer@example.com');
    await page.fill('#Password', 'password123');
    await page.click('#LoginButton');

    // Navigate to claims page and submit a new claim
    await page.goto('http://localhost:5000/Claims/Create');
    await page.fill('#HoursWorked', '10');
    await page.fill('#HourlyRate', '150');
    await page.click('#SubmitButton');

    // Generate and download claims report
    await page.click('#GenerateClaimsReportButton');
    const [download] = await Promise.all([
        page.waitForEvent('download'),
        page.click('#DownloadClaimsLink')
    ]);

    const content = await download.text();
    expect(content).toContain('Claim ID');
    expect(content).toContain('lecturer@example.com');
});
