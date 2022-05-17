const DEFAULT_OPTIONS = {
  html: false,
  classPrefix: 'sql-hl-',
  colors: {
    keyword: '\x1b[35m',
    function: '\x1b[31m',
    number: '\x1b[32m',
    string: '\x1b[32m',
    special: '\x1b[33m',
    bracket: '\x1b[33m',
    clear: '\x1b[0m'
  }
}

const keywords = [
    'ADD',
    'ADD CONSTRAINT',
    'ALTER',
    'ALTER COLUMN',
    'ALTER TABLE',
    'ALL',
    'AND',
    'ANY',
    'AS',
    'ASC',
    'BACKUP DATABASE',
    'BETWEEN',
    'CASE',
    'CHECK',
    'COLUMN',
    'CONSTRAINT',
    'CREATE',
    'CREATE DATABASE',
    'CREATE INDEX',
    'CREATE OR REPLACE VIEW',
    'CREATE TABLE',
    'CREATE PROCEDURE',
    'CREATE UNIQUE INDEX',
    'CREATE VIEW',
    'DATABASE',
    'DEFAULT',
    'DELETE',
    'DESC',
    'DISTINCT',
    'DROP',
    'DROP COLUMN',
    'DROP CONSTRAINT',
    'DROP DATABASE',
    'DROP DEFAULT',
    'DROP INDEX',
    'DROP TABLE',
    'DROP VIEW',
    'EXEC',
    'EXISTS',
    'FOREIGN KEY',
    'FROM',
    'FULL OUTER JOIN',
    'GROUP BY',
    'HAVING',
    'IN',
    'INDEX',
    'INNER JOIN',
    'INSERT INTO',
    'INSERT INTO SELECT',
    'IS NULL',
    'IS NOT NULL',
    'JOIN',
    'LEFT JOIN',
    'LIKE',
    'LIMIT',
    'NOT',
    'NOT NULL',
    'OR',
    'ORDER BY',
    'OUTER JOIN',
    'PRIMARY KEY',
    'PROCEDURE',
    'RIGHT JOIN',
    'ROWNUM',
    'SELECT',
    'SELECT DISTINCT',
    'SELECT INTO',
    'SELECT TOP',
    'SET',
    'TABLE',
    'TOP',
    'TRUNCATE TABLE',
    'UNION',
    'UNION ALL',
    'UNIQUE',
    'UPDATE',
    'VALUES',
    'VIEW',
    'WHERE',
    'PRAGMA',
    'INTEGER',
    'PRIMARY',
    'letCHAR',
    'DATETIME',
    'NULL',
    'REFERENCES',
    'INDEX_LIST',
    'BY',
    'CURRENT_DATE',
    'CURRENT_TIME',
    'EACH',
    'ELSE',
    'ELSEIF',
    'FALSE',
    'FOR',
    'GROUP',
    'IF',
    'INSERT',
    'INTERVAL',
    'INTO',
    'IS',
    'KEY',
    'KEYS',
    'LEFT',
    'MATCH',
    'ON',
    'OPTION',
    'ORDER',
    'OUT',
    'OUTER',
    'REPLACE',
    'TINYINT',
    'RIGHT',
    'THEN',
    'TO',
    'TRUE',
    'WHEN',
    'WITH',
    'UNSIGNED',
    'CASCADE',
    'ENGINE',
    'TEXT',
    'AUTO_INCREMENT',
    'SHOW',
    'BEGIN',
    'END',
    'PRINT',
    'OVERLAPS'
]

const SPLIT_CHARS = '[^a-zA-Z_]'

const highlighters = [
  {
    name: 'keyword',
    group: 1,
    regex: new RegExp(`(^|${SPLIT_CHARS})(${keywords.join('|')}|${keywords.map(x => x.toLowerCase()).join('|')})(?=${SPLIT_CHARS}|$)`, 'g')
  },
  {
    name: 'special',
    regex: /(=|!=|%|\/|\*|-|,|;|:|\+|<|>)/g
  },
  {
    name: 'function',
    regex: /(\w+?)\(/g,
    trimEnd: 1
  },
  {
    name: 'number',
    regex: /(\d+(?:\.\d+)?)/g
  },
  {
    name: 'string',
    regex: /(["'`].*?["'`])/g
  },
  {
    name: 'bracket',
    regex: /([()])/g
  }
]

function highlight (sqlString, options) {
  options = Object.assign({}, DEFAULT_OPTIONS, options)

  const matches = []

  for (const hl of highlighters) {
    let match

    // This is probably the one time when an assignment inside a condition makes sense
    // eslint-disable-next-line no-cond-assign
    while (match = hl.regex.exec(sqlString)) {
      let text = match[0]
      let boringLength = 0

      // If a specific group is requested, use that group instead, and make sure
      // we offset the index by the length of the preceding groups
      if (hl.group) {
        text = match[hl.group + 1]
        for (let i = 1; i <= hl.group; i++) {
          boringLength += match[i].length
        }
      }

      matches.push({
        name: hl.name,
        start: match.index + boringLength,
        length: (hl.trimEnd ? text.substr(0, text.length - hl.trimEnd) : text).length
      })
    }
  }

  const sortedMatches = matches.slice().sort((a, b) => a.start - b.start)

  // filter/exclude nested matches (matches within the last match)
  const filteredMatches = []
  let upperBound = 0
  for (let i = 0; i < sortedMatches.length; i++) {
    if (sortedMatches[i].start >= upperBound) {
      filteredMatches.push(sortedMatches[i])
      upperBound = sortedMatches[i].start + sortedMatches[i].length
    }
  }

  let highlighted = ''

  for (let i = 0; i < filteredMatches.length; i++) {
    const match = filteredMatches[i]
    const nextMatch = filteredMatches[i + 1]

    const stringMatch = sqlString.substr(match.start, match.length)

    if (options.html) {
      highlighted += `<span class="${options.classPrefix}${match.name}">`
      highlighted += stringMatch
      highlighted += '</span>'
    } else {
      highlighted += options.colors[match.name]
      highlighted += stringMatch
      highlighted += options.colors.clear
    }
    if (nextMatch) {
      highlighted += sqlString.substr(match.start + match.length, nextMatch.start - (match.start + match.length))
    } else if (sqlString.length > (match.start + match.length)) {
      highlighted += sqlString.substr(match.start + match.length)
    }
  }

  console.log(highlighted);
  return highlighted
}
