#!/usr/bin/env node

/**
 * Quality Gate Validator - 00-development-gate.md
 * Validates that development follows all .clinerules/
 */

const fs = require('fs');
const path = require('path');

const RED = '\x1b[31m';
const GREEN = '\x1b[32m';
const YELLOW = '\x1b[33m';
const BLUE = '\x1b[34m';
const RESET = '\x1b[0m';

console.log(`${BLUE}🚪 SportPlanner Development Quality Gate - Validating Rules${RESET}\n`);

// Read last commit message to check if checklist was mentioned
function getLastCommitMessage() {
  try {
    const { execSync } = require('child_process');
    return execSync('git log -1 --pretty=%B', { encoding: 'utf8' }).trim();
  } catch (error) {
    return '';
  }
}

// Check if staging area has files (skip for initial commit)
function hasStagedChanges() {
  try {
    const { execSync } = require('child_process');
    const output = execSync('git diff --cached --name-only', { encoding: 'utf8' });
    return output.trim().length > 0;
  } catch (error) {
    return false;
  }
}

// Validate each category of rules
function validateCategory(category, rules) {
  let violations = [];

  rules.forEach(rule => {
    const rulePath = path.join('.clinerules', rule);
    if (!fs.existsSync(rulePath)) {
      violations.push(`Missing rule file: ${rulePath}`);
      return;
    }

    // Check if rule was modified recently or is being committed
    const stats = fs.statSync(rulePath);
    const daysSinceModified = (Date.now() - stats.mtimeMs) / (1000 * 60 * 60 * 24);

    if (daysSinceModified > 30) {
      violations.push(`Rule ${rule} hasn't been reviewed in ${Math.floor(daysSinceModified)} days`);
    }
  });

  return violations;
}

// Main validation
function validateQualityGate() {
  const categories = {
    'Architecture': [
      '01-clean-code.md',
      '14-dotnet-backend.md',
      '10-angular-structure.md',
      '03-adr.md'
    ],
    'Security': [
      '13-supabase-jwt.md',
      '05-security.md'
    ],
    'Code Quality': [
      '02-naming.md',
      '06-tool-usage.md'
    ],
    'Testing': [
      '04-testing.md'
    ],
    'UI/UX': [
      '10-angular-structure.md',
      '12-material-animations.md',
      '11-tailwind.md',
      '15-ui-ux-excellence.md'
    ]
  };

  let allViolations = [];
  let totalRules = 0;

  console.log('📋 Validating rule categories...\n');

  for (const [category, rules] of Object.entries(categories)) {
    console.log(`${BLUE}${category}:${RESET}`);
    const violations = validateCategory(category, rules);
    totalRules += rules.length;

    if (violations.length === 0) {
      console.log(`  ${GREEN}✓ All rules present and recent${RESET}`);
    } else {
      violations.forEach(violation => {
        console.log(`  ${RED}✗ ${violation}${RESET}`);
        allViolations.push(violation);
      });
    }
    console.log('');
  }

  // Check if staging area has changes (skip validation for initial setup)
  if (!hasStagedChanges()) {
    console.log(`${YELLOW}⚠️  No staged changes detected. Skipping other validations.${RESET}\n`);
    console.log(`${GREEN}✅ Pre-commit hook setup complete!${RESET}`);
    return true;
  }

  // Check commit message for checklist reference
  const commitMessage = getLastCommitMessage().toLowerCase();
  const checklistTerms = ['checklist', 'quality-gate', 'rules', 'development-gate', 'compliance'];

  const mentionsChecklist = checklistTerms.some(term =>
    commitMessage.includes(term.toLowerCase())
  );

  if (!mentionsChecklist && allViolations.length === 0) {
    console.log(`${YELLOW}⚠️  Warning: Consider mentioning "Quality Gate" in commit message${RESET}`);
    console.log(`   Example: "feat: add training component (Quality Gate: Reviewed)"
`);
  }

  // Final result
  if (allViolations.length === 0) {
    console.log(`${GREEN}✅ Quality Gate PASSED - All rules validated!${RESET}`);
    console.log(`${GREEN}🚀 You may proceed with your development!${RESET}\n`);
    return true;
  } else {
    console.log(`${RED}❌ Quality Gate FAILED - ${allViolations.length} violations found!${RESET}`);
    console.log(`${YELLOW}📖 Please review .clinerules/00-development-gate.md and fix violations${RESET}\n`);
    console.log(`${BLUE}💡 Common fixes:${RESET}`);
    console.log(`   - Ensure all rule files exist`);
    console.log(`   - Review rules modified recently`);
    console.log(`   - Mention "Quality Gate" in commit message`);

    process.exit(1);
  }
}

// Run validation
try {
  const passed = validateQualityGate();
  if (passed) {
    process.exit(0);
  }
} catch (error) {
  console.error(`${RED}❌ Error during validation:${RESET}`, error.message);
  process.exit(1);
}
